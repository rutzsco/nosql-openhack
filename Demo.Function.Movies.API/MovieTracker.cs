using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Demo.Function.Movies.API.Data;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Demo.Function.Movies.API
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MovieTracker
    {
        private readonly ExecutionContext _ExecutionContext;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger _Logger;

        public MovieTracker(ExecutionContext executionContext, ILogger logger)
        {
            _ExecutionContext = executionContext;
            _Logger = logger;
        }

        [JsonProperty("buyCount")]
        public long BuyCount { get; set; }


        public void ProcessNewOrderDetail(OrderDetail order)
        {
            _Logger.LogInformation($"ProcessNewOrder - Begin Processing. Id {order.ProductId}");
            BuyCount = BuyCount + Convert.ToInt32(order.Quantity);
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

        [FunctionName("MovieTracker")]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx, ExecutionContext executionContext, ILogger logger)
            => ctx.DispatchAsync<MovieTracker>(executionContext, logger);
    }
}