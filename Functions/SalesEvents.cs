using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace BFYOC
{
    public static class SalesEvents
    {
    
    [FunctionName("SalesEvents")]
    [return: ServiceBus("receipt", Connection = "myServiceBus")]
        public static async Task Run(
        [EventHubTrigger(
            "pointofsales", 
            Connection = "myEventHub")] 
            string[] events, 
        [CosmosDB( 
            databaseName: "RatingsAPI",
            collectionName: "Sales",
            ConnectionStringSetting = "myCosmosDb")]
            IAsyncCollector<dynamic> SaleEvent,
        [ServiceBus(
            "receipt", 
            Connection = "MyServiceBus")] 
            IAsyncCollector<dynamic> queueCollectorLarge,
        [ServiceBus(
            "receiptsmall", 
            Connection = "MyServiceBus")] 
            IAsyncCollector<dynamic> queueCollectorSmall,
        ILogger log)
        {
            
            var exceptions = new List<Exception>();

            foreach (var eventData in events)
            {
                try
                {
                    //Send documents to CosmosDB
                    await SaleEvent.AddAsync(eventData);
                    log.LogInformation("Added sale event to Cosmos");

                    //convert eventData to json
                    log.LogInformation("Converting event to JSON...");
                    dynamic json = JsonConvert.DeserializeObject(eventData);
                    log.LogInformation($"Event converted to json...output: {json}");

                    //If receiptUrl exists, send to Service Bus queue
                    var receiptUrl = json.header.receiptUrl;
                    log.LogInformation($"receiptUrl value is {receiptUrl}");
                    

                    if (receiptUrl != null)
                    {
                        log.LogInformation("Receipt URL exists...");
                        
                        //Count number of total items in the sale.
                        int items = 0;
                        foreach (var item in json.details) 
                        {
                            items = items + 1;
                        }
                        log.LogInformation($"{items} items found...");

                        //Create receipt object
                        var rating = new Receipt {
                            totalItems = items,
                            totalCost = json.header.totalCost,
                            salesNumber = json.header.salesNumber,
                            salesDate = json.header.dateTime,
                            storeLocation = json.header.locationId,
                            receiptUrl = json.header.receiptUrl
                         };

                        //Convert rating Object to JSON
                        var ratingJSON = Newtonsoft.Json.JsonConvert.SerializeObject(rating);

                        //Add object to Service Bus based on totalCost amount
                        if (json.header.totalCost >100) {
                            log.LogInformation($"Adding large receipt event to Service Bus queue...{ratingJSON}");
                            await queueCollectorLarge.AddAsync(ratingJSON);
                        }
                        else {
                            log.LogInformation($"Adding small receipt event to Service Bus queue...{ratingJSON}");
                            await queueCollectorSmall.AddAsync(ratingJSON);
                        }

                    }
                    else {
                        log.LogInformation("No receipt URL found for sale event.");
                    }

                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }
            //Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.
            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);
            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
