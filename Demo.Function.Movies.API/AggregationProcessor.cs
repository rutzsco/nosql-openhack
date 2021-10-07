using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Demo.Function.Movies.Api.Data;
using Demo.Function.Movies.API.Data;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace Demo.Function.Movies.API
{
    public class AggregationProcessor
    {
        private CosmosClient _cosmosClient;

        public AggregationProcessor(CosmosClient cosmosClient)
        {
            this._cosmosClient = cosmosClient;
        }

        [FunctionName("AggregationProcessor")]
        public async Task Run([EventHubTrigger("telemetry", Connection = "IngestEventHubConnectionString", ConsumerGroup = "scrutz")] EventData[] events, [DurableClient] IDurableClient context, ILogger log)
        {         
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var order = JsonSerializer.Deserialize<Order>(messageBody);

                foreach (var orderDetail in order.Details)
                {
                    orderDetail.OrderDate = order.OrderDate;

                    await context.SignalEntityAsync(new EntityId("MovieHourTracker1", $"{Convert.ToString(orderDetail.ProductId)}-{orderDetail.OrderDate.Year}-{orderDetail.OrderDate.Month}-{orderDetail.OrderDate.Day}-{orderDetail.OrderDate.Hour}"), "ProcessNewOrderDetail", orderDetail);
                    await context.SignalEntityAsync(new EntityId("MovieTracker5", Convert.ToString(orderDetail.ProductId)), "ProcessNewOrderDetail", orderDetail);
                    
                    var queryService = new MovieQueryService(_cosmosClient);
                    var item = await queryService.GetById(Convert.ToString(orderDetail.ProductId));

                    orderDetail.CategoryId = item.Result.CategoryId;
                    await context.SignalEntityAsync(new EntityId("CategoryTracker1", orderDetail.CategoryId), "ProcessNewOrderDetail", orderDetail);

                    orderDetail.AggregationId = $"{Convert.ToString(orderDetail.CategoryId)}-{orderDetail.OrderDate.Year}-{orderDetail.OrderDate.Month}-{orderDetail.OrderDate.Day}";
                    await context.SignalEntityAsync(new EntityId("CategoryDayTracker1", orderDetail.AggregationId), "ProcessNewOrderDetail", orderDetail);
                }
            }
        }
    }
}
