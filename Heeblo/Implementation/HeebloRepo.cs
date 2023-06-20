
using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using ProWritingAid.SDK.Api;
using ProWritingAid.SDK.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Heeblo.Implementation
{
    public class HeebloRepo : IHeeblo
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public HeebloRepo(IConfiguration config, ApplicationDbContext db)
        {
            this._config = config;
            this._db = db;
        }

        public async Task<string> Plagiarism(string content)
        {
            string apiKey = _config["pKey"];
            string textToCheck = content;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.check-plagiarism.com/apis/");

                var requestData = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("key", apiKey),
            new KeyValuePair<string, string>("data", textToCheck)
            });

                try
                {
                    HttpResponseMessage response = client.PostAsync("checkPlag", requestData).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        var responseObj = JsonConvert.DeserializeObject<dynamic>(result);
                        int plagiarismPercent = responseObj.plagPercent;
                        return plagiarismPercent.ToString();
                    }
                    else
                    {
                        return "Request failed with status code: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    return "An error occurred: " + ex.Message;
                }
            }
        }
        public async Task<string> AiDetect(string content)
        {
            string apiUrl = "https://api.sapling.ai/api/v1/aidetect";

            // Set your API key
            string apiKey = _config["aiKey"];

            // Set the text to run detection on
            string text = content;

            // Set sent_scores parameter to false
            bool sentScores = false;

            // Create the request payload
            var requestPayload = new
            {
                key = apiKey,
                text = text,
                sent_scores = sentScores
            };

            // Send the POST request to the API
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PostAsJsonAsync(apiUrl, requestPayload).Result;
                    response.EnsureSuccessStatusCode();

                    // Read the response as JSON
                    string result = response.Content.ReadAsStringAsync().Result;
                    var responseBody = JsonConvert.DeserializeObject<dynamic>(result);

                    var score = responseBody.score;

                    return score.ToString();
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }
        public async Task<string> Grammer(string content)
        {
            var api = new TextAsyncApi().SetLicenseCode(_config["gKey"]);
            var request = new TextAnalysisRequest(
                content,
                new List<string> { "grammar" },
                TextAnalysisRequest.StyleEnum.General,
                TextAnalysisRequest.LanguageEnum.En);

            try
            {
                var response = api.PostAsync(request).Result;
                //int a = response.Summaries?.FirstOrDefault()?.NumberOfIssues??0;
                string a = response.Summaries?.FirstOrDefault()?.SummaryItems?.FirstOrDefault()?.Text ?? "";
                string score = GetPercentValue(a);
                return score;
            }

            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        public string GetPercentValue(string str)
        {
            string input = str;

            string pattern = @"(\d+)%";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string scoreStr = match.Groups[1].Value;
                int score = int.Parse(scoreStr);

                return (Convert.ToString(score));
            }
            else
            {
                return "";
            }
        }
        public async Task GetScores(string content, int id)
        {
            try
            {
                string connectionString = _config.GetConnectionString("HBL");
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    decimal plagiarism = Math.Round(Decimal.Parse(Plagiarism(content).Result), 2);
                    decimal ai_score = Math.Round(Decimal.Parse(AiDetect(content).Result), 2)*100;
                    decimal grammar_score = Math.Round(Decimal.Parse(Grammer(content).Result), 2);

                    string sql = "UPDATE hbl_tbl_application SET plagiarism = @plagiarism, ai_score = @ai_score,grammar_score = @grammar_score WHERE application_id = @id";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@plagiarism", plagiarism);
                    command.Parameters.AddWithValue("@ai_score", ai_score);
                    command.Parameters.AddWithValue("@grammar_score", grammar_score);
                    command.Parameters.AddWithValue("@id", id);
                    int rowsAffected = command.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public async Task<bool> SendEmail(string emaiId,string subject,string body)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(emaiId);
            mail.From = new MailAddress(_config.GetValue<string>("Mail:From"));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = _config.GetValue<string>("Mail:Server");
            smtp.Port = _config.GetValue<int>("Mail:Port");
            smtp.Credentials = new NetworkCredential(
                _config.GetValue<string>("Mail:From"),
                _config.GetValue<string>("Mail:Password")
                );
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return true;
            
        }
    }
}
