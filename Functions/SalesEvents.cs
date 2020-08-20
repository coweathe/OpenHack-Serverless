using System;
// using System.IO;
// using System.Collections.Generic;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
// using Microsoft.Azure.Cosmos;
// using Microsoft.Azure.Documents;
// using Microsoft.Azure.Documents.Linq;
// using Microsoft.Azure.WebJobs.Extensions.Http;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Azure.WebJobs.Host;
using Azure.Messaging.EventHubs;
// using Azure.Messaging.EventHubs.Consumer;
// using Azure.Messaging.EventHubs.Producer;
// using Azure.Storage.Blobs;
// using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
//using Microsoft.Azure.WebJobs.Extensions.EventHubs;
//using Microsoft.Azure.Documents.Client;
// using Newtonsoft.Json;
using System.Net.Http;
//using System.Collections.Generic;
//using System.Linq;
// using System.Text;

namespace BFYOC
{
    public static class SalesEvents
    {
        //private static readonly HttpClient client = new HttpClient();

        [FunctionName("SalesEvents")]
        public static async Task Run(
            [EventHubTrigger("pointofsales", Connection = "myEventHub")] EventData[] eventHubMessages, 
            [CosmosDB( 
                databaseName: "RatingsAPI",
                collectionName: "Sales", 
                ConnectionStringSetting = "myCosmosDb")]
                IAsyncCollector<EventData> SaleEvent,
            ILogger log)
        {

            try
            {
                log.LogInformation($"C# Queue trigger function processed {eventHubMessages?.Length} items");

                foreach (EventData message in eventHubMessages)
                {
                    log.LogInformation($"Description={message}");
                    await SaleEvent.AddAsync(message);
                }
            }

            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
