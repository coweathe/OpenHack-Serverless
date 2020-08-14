using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
//using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Net.Http;
//using System.Collections.Generic;
//using System.Linq;

namespace BFYOC
{

    /*
    ---------------INPUT-------------------
    {
        "ratingId": "79c2779e-dd2e-43e8-803d-ecbebed8972c"
    }

    ---------------OUTPUT-------------------
    {
    "id": "79c2779e-dd2e-43e8-803d-ecbebed8972c",
    "userId": "cc20a6fb-a91f-4192-874d-132493685376",
    "productId": "4c25613a-a3c2-4ef3-8e02-9c335eb23204",
    "timestamp": "2018-05-21 21:27:47Z",
    "locationName": "Sample ice cream shop",
    "rating": 5,
    "userNotes": "I love the subtle notes of orange in this ice cream!"
    }
    */

    public static class GetRating
    {

        //Reusable instance of ItemClient which represents the connection to a Cosmos endpoint
        private static Container container = null;

        private static readonly HttpClient client = new HttpClient();

        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ratings/{ratingId}")] HttpRequest req,
            [CosmosDB( 
                databaseName: "RatingsAPI", 
                collectionName: "Container1", 
                ConnectionStringSetting = "myCosmosDb",
                Id = "{ratingId}",
                PartitionKey = "{ratingId}")] Rating rating,
            ILogger log)
        
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                // Grab data from body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                //log.LogInformation($"ratingId: {data.ratingId} ");
 
                if(rating == null) 
                {
                    log.LogInformation($"Rating not found");

                    //Return error message

                }
                else
                {
                    log.LogInformation($"Found rating, productId={rating.productId}");

                }
                return new OkObjectResult(rating);
            }
            
            catch(Exception e)
            {
                Console.WriteLine(e);
                return (ActionResult)new BadRequestObjectResult("Error");
            }
        }
    }
}
