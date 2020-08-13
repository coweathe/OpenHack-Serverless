using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

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
                userNotes = data.userNotes
            };

            await document.AddAsync(rating);

            return rating;

        }
    }
}