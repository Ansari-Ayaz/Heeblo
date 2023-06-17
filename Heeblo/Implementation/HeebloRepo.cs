
using Heeblo.Repository;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Heeblo.Implementation
{
    public class HeebloRepo : IHeeblo
    {
        public async Task Plagiarism()
        {
            string apiKey = "6f5a1ada7d24d7739c6b1e907645ed49";
            string textToCheck = "I am checking plagiarism ........... black permanent marker.";

            using (HttpClient client = new HttpClient())
            {
                // Set the base URL of the API
                client.BaseAddress = new Uri("https://www.check-plagiarism.com/apis/");

                // Prepare the request data
                var requestData = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("key", apiKey),
                new KeyValuePair<string, string>("data", textToCheck)
            });

                try
                {
                    // Send the POST request to the API
                    HttpResponseMessage response = await client.PostAsync("checkPlag", requestData);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        string result = await response.Content.ReadAsStringAsync();

                        // Deserialize the response JSON
                        var responseObj = JsonConvert.DeserializeObject<dynamic>(result);

                        // Extract the plagiarism percentage
                        int plagiarismPercent = responseObj.plagPercent;

                        // Display the plagiarism percentage
                        Console.WriteLine("Plagiarism percentage: " + plagiarismPercent + "%");
                    }
                    else
                    {
                        // Display the error message if the request failed
                        Console.WriteLine("Request failed with status code: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    // Display any exception that occurred
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
        public async Task AiDetect()
        {
            string apiUrl = "https://api.sapling.ai/api/v1/aidetect";

            // Set your API key
            string apiKey = "YOUR_API_KEY";

            // Set the text to run detection on
            string text = "Text to run detection on. This is an example text.";

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
                    var response = await client.PostAsJsonAsync(apiUrl, requestPayload);
                    response.EnsureSuccessStatusCode();

                    // Read the response as JSON
                    var responseBody = await response.Content.ReadFromJsonAsync<dynamic>();

                    // Extract the score
                    double score = responseBody["score"];

                    // Print the score
                    Console.WriteLine("Score: " + score);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }



        }
    }
}
