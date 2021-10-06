using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Demo.Function.Movies.API.Data;

using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Demo.Function.Movies.API
{
    public static class AggregationProcessor
    {
        [FunctionName("AggregationProcessor")]
        public static async Task Run([CosmosDBTrigger(databaseName: "MoviesDB", collectionName: "TBD", ConnectionStringSetting = "", LeaseCollectionName = "leases")]IReadOnlyList<Document> input, [DurableClient] IDurableClient context, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                foreach (var document in input)
                {
                    var order = JsonConvert.DeserializeObject<Order>(document.ToString());
                    var entityId = new EntityId("MovieTracker", order.id.ToString());
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        await context.SignalEntityAsync(entityId, "ProcessNewOrderDetail", orderDetail);
                    }
                }
            }
        }
    }
}
