using System;
using System.Collections.Generic;
using System.Linq;

using Demo.Function.Movies.Api.Data;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using StackExchange.Redis;

namespace Demo.Function.Movies.API
{
    public class CategoryChangeFeedActivity
    {
        private IConnectionMultiplexer _redisConnectionMultiplexer;
        private CosmosClient _cosmosClient;

        public CategoryChangeFeedActivity(IConnectionMultiplexer redisConnectionMultiplexer, CosmosClient cosmosClient)
        {
            this._redisConnectionMultiplexer = redisConnectionMultiplexer;
            _cosmosClient = cosmosClient;
        }

        [FunctionName("CategoryChangeFeedActivity")]
        public void Run([CosmosDBTrigger(databaseName: "MoviesDB", collectionName: "Category", ConnectionStringSetting = "CosmosDBConnection", LeaseCollectionName = "leases", StartFromBeginning = true)] IReadOnlyList<Document> input, ILogger log)
        {
            if (input == null || input.Count <= 0)
                return;

            var qs = new MovieQueryService(_cosmosClient);
            var activeCategories = qs.GetCategories().Result.Result.Where(x => x.IsActive);

            var db = _redisConnectionMultiplexer.GetDatabase();
            db.StringSet("CategoryCache", JsonConvert.SerializeObject(activeCategories));
        }
    }
}
