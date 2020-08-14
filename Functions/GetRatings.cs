using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
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

    public static class GetRatings
    {

        private static readonly HttpClient client = new HttpClient();

        [FunctionName("GetRatings")]
         public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ratings/{userId}")] HttpRequest req,
            [CosmosDB( 
                databaseName: "RatingsAPI",
                collectionName: "Container1", 
                ConnectionStringSetting = "myCosmosDb",
                SqlQuery = "SELECT * FROM c WHERE c.userId = '{userId}'")] IEnumerable<Rating> ratings,
            ILogger log,
            string userId)
        
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                
                
                //Get data from body
                //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                //dynamic data = JsonConvert.DeserializeObject(requestBody);

                if (ratings == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(ratings);
                
            }

            
            catch(Exception e)
            {
                Console.WriteLine(e);
                return (ActionResult)new BadRequestObjectResult("Error");
            }
        }
    }
}













/* ------------------DOCUMENTCLIENT METHOD---------------------
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ratings/{userId}")] HttpRequest req,
            [CosmosDB( 
                databaseName: "RatingsAPI",
                collectionName: "Container1", 
                ConnectionStringSetting = "myCosmosDb")] DocumentClient client,
            ILogger log)
      {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var userId = req.Query["{userId}"];
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return (ActionResult)new NotFoundResult();
                }

                Uri collectionUri = UriFactory.CreateDocumentCollectionUri("ToDoItems", "Items");

                log.LogInformation($"Searching for: {userId}");

                IDocumentQuery<Rating> query = client.CreateDocumentQuery<Rating>(collectionUri)
                    .Where(p => p.Description.Contains(searchterm))
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    foreach (Rating result in await query.ExecuteNextAsync())
                    {
                        log.LogInformation(result.userId);
                    }
                }
                return new OkResult();
*/




/* ------------------SQL QUERY METHOD---------------------
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ratings/{userId}")] HttpRequest req,
            [CosmosDB( 
                databaseName: "RatingsAPI",
                collectionName: "Container1", 
                ConnectionStringSetting = "myCosmosDb",
                SqlQuery = "SELECT * FROM c WHERE c.userId = '{userId}'")] IEnumerable<Rating> ratings,
            ILogger log)
        
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                
                
                //Get data from body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                foreach (Rating rating in ratings) 
                {
                    log.LogInformation(rating.id.ToString());
                }

                return new OkObjectResult(ratings);
*/