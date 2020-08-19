using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
// using Microsoft.Azure.Cosmos;
// using Microsoft.Azure.Documents;
// using Microsoft.Azure.Documents.Linq;
// using Microsoft.Azure.WebJobs.Extensions.Http;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Azure.WebJobs.Host;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
// using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
//using Microsoft.Azure.WebJobs.Extensions.EventHubs;
//using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Net.Http;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace BFYOC
{
    public static class SalesEvents
    {
        private static readonly HttpClient client = new HttpClient();

        [FunctionName("SalesEvents")]
        public static void Run(
            [EventHubTrigger("pointofsales", Connection = "myEventHub")] EventData[] eventHubMessages, 
            [CosmosDB( 
                databaseName: "RatingsAPI",
                collectionName: "Sales", 
                ConnectionStringSetting = "myCosmosDb")]out dynamic document,
            ILogger log)
        {
            document = null;
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                foreach (var message in eventHubMessages)
                {
                    document = new { Description = message, id = Guid.NewGuid() };
                    
                    log.LogInformation($"C# Queue trigger function inserted one row");
                    log.LogInformation($"Description={message}");
                }
            }

            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
