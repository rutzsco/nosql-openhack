using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Demo.Function.Movies.API.Data;

using Microsoft.Azure.Documents;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace Demo.Function.Movies.API
{
    public static class AggregationProcessor
    {
        [FunctionName("AggregationProcessor")]
        public static async Task Run([EventHubTrigger("telemetry", Connection = "IngestEventHubConnectionString", ConsumerGroup = "scrutz")] EventData[] events, [DurableClient] IDurableClient context, ILogger log)
        {
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var order = JsonSerializer.Deserialize<Order>(messageBody);

                foreach (var orderDetail in order.Details)
                {
                    orderDetail.OrderDate = order.OrderDate;

                    var entityId = new EntityId("MovieTracker3", $"{Convert.ToString(orderDetail.ProductId)}-{orderDetail.OrderDate.Year}-{orderDetail.OrderDate.Month}-{orderDetail.OrderDate.Day}-{orderDetail.OrderDate.Hour}");
                    await context.SignalEntityAsync(entityId, "ProcessNewOrderDetail", orderDetail);
                }
            }
        }
    }
}
