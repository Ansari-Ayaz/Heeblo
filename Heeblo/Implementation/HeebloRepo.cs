
using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProWritingAid.SDK.Api;
using ProWritingAid.SDK.Model;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Heeblo.Implementation
{
    public class HeebloRepo : IHeeblo
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public HeebloRepo(IConfiguration config,ApplicationDbContext db)
        {
            this._config = config;
            this._db = db;
        }

        public string Plagiarism(string content)
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
        public string AiDetect(string content)
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

                    var score = responseBody;

                    return score.ToString();
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }
        public string Grammer(string content)
        {
            var api = new TextAsyncApi().SetLicenseCode(_config["gKey"]);
            var request = new TextAnalysisRequest(
                content,
                new List<string> { "grammar" },
                TextAnalysisRequest.StyleEnum.General,
                TextAnalysisRequest.LanguageEnum.En);

            try
            {
                var response = api.Post(request);
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

                return ("Score: " + score + "%");
            }
            else
            {
                return "";
            }
        }
        public async Task GetScores(string content,int id)
        {
            var obj = _db.hbl_tbl_application.FirstOrDefault(z => z.application_id.Equals(id));
            obj.plagiarism =  decimal.Parse(Plagiarism(content));
            obj.ai_score = decimal.Parse(AiDetect(content));
            obj.grammar_score = decimal.Parse(Grammer(content));
            
            _db.hbl_tbl_application.Attach(obj);
            _db.Entry(obj).State = EntityState.Modified;
            _db.SaveChanges();
        }

    }
}
