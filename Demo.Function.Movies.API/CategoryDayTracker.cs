using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Demo.Function.Movies.API.Data;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Demo.Function.Movies.API
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CategoryDayTracker
    {
        private readonly ILogger _Logger;
        private CosmosClient _cosmosClient;

        public CategoryDayTracker(CosmosClient cosmosClient, ILogger logger)
        {
            _Logger = logger;
            _cosmosClient = cosmosClient;
        }

        [JsonProperty("buyCount")]
        public long BuyCount { get; set; }

        public void ProcessNewOrderDetail(OrderDetail order)
        {
            _Logger.LogInformation($"ProcessNewOrder - Begin Processing. Id {order.CategoryId}");
            BuyCount = BuyCount + Convert.ToInt32(order.Quantity);

            var c1 = new
            {
                id = order.AggregationId,
                productId = order.CategoryId,
                buyCount = BuyCount,
                type = "CategoryDayBuyCount",
                dateTime = new DateTime(order.OrderDate.Year, order.OrderDate.Month, order.OrderDate.Day, order.OrderDate.Hour, 0, 0)
            };
            var container = _cosmosClient.GetContainer("MoviesDB", "MovieStatistics");
            container.UpsertItemAsync(c1, new PartitionKey(c1.id)).Wait();
        }

        public Task Reset()
        {
            this.BuyCount = 0;
            return Task.CompletedTask;
        }

        public Task<long> Get()
        {
            return Task.FromResult(this.BuyCount);
        }
        public void Delete()
        {
            Entity.Current.DeleteState();
        }

        [FunctionName("CategoryDayTracker1")]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx, ExecutionContext executionContext, ILogger logger)
            => ctx.DispatchAsync<CategoryDayTracker>(executionContext, logger);
    }
}