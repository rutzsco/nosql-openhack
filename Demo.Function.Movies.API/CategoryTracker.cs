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
    public class CategoryTracker
    {
        private CosmosClient _cosmosClient;

        public CategoryTracker(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        [JsonProperty("buyCount")]
        public long BuyCount { get; set; }

        public void ProcessNewOrderDetail(OrderDetail order)
        {
            BuyCount = BuyCount + Convert.ToInt32(order.Quantity);

            var container = _cosmosClient.GetContainer("MoviesDB", "MovieStatistics");
            var m = new
            {
                id = order.CategoryId,
                categoryId = order.CategoryId,
                buyCount = BuyCount,
                type = "CategoryBuyCount",
                dateTime = new DateTime(order.OrderDate.Year, order.OrderDate.Month, order.OrderDate.Day, order.OrderDate.Hour, 0, 0)
            };
            container.UpsertItemAsync(m, new PartitionKey(m.id)).Wait();
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

        [FunctionName("CategoryTracker1")]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<CategoryTracker>();
    }
}