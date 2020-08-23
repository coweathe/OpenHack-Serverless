using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace BFYOC {
    public class CosmosService {
        public async Task<Rating> CreateRatingFromDocument(dynamic data, IAsyncCollector<Rating> document, HttpClient client, ILogger logger) {
            //Set unique id
            var id = Guid.NewGuid();

            var postData = new {
                documents = new[] {
                    new {
                        language = "en",
                        id = id,
                        text = data.userNotes
                    }
                }
            };
            
            //Serialize JSON
            var postDataJSON = JsonConvert.SerializeObject(postData);
            
            //Build HTTP request
            var request = new HttpRequestMessage(HttpMethod.Post, "https://textanalytics-bfyoc.cognitiveservices.azure.com/text/analytics/v2.1/sentiment");            
            request.Headers.Add("Ocp-Apim-Subscription-Key","90820d14c8384068b220d28f425991a5");
            request.Content = new StringContent(postDataJSON, Encoding.UTF8, "application/json");
            
            //Send HTTP POST request
            var response = await client.SendAsync(request);
            
            //Create JSON string from http response
            string responseContent = response.Content.ReadAsStringAsync().Result;
            //Deserialize JSON string
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseContent);
            //Get sentiment score value from response
            var sentimentScore = myDeserializedClass.documents[0].score;

            //Set timestamp
            var timestamp = DateTime.UtcNow;

            var rand = new Random();

            var rating = new Rating {
                id = id,
                userId = data.userId,
                productId = data.productId,
                timestamp = timestamp,
                locationName = data.locationName,
                rating = data.rating,
                userNotes = data.userNotes,
                sentimentScore = sentimentScore
            };

            await document.AddAsync(rating);

            return rating;

        }
    }
}