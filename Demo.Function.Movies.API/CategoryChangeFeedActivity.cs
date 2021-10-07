using System;
using System.Collections.Generic;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using StackExchange.Redis;

namespace Demo.Function.Movies.API
{
    public class CategoryChangeFeedActivity
    {
        private IConnectionMultiplexer _redisConnectionMultiplexer;

        public CategoryChangeFeedActivity(IConnectionMultiplexer redisConnectionMultiplexer)
        {
            this._redisConnectionMultiplexer = redisConnectionMultiplexer;
        }

        [FunctionName("CategoryChangeFeedActivity")]
        public void Run([CosmosDBTrigger(databaseName: "MoviesDB", collectionName: "Category", ConnectionStringSetting = "CosmosDBConnection", LeaseCollectionName = "leases")] IReadOnlyList<Document> input, ILogger log)
        {
            if (input == null || input.Count <= 0)
                return;

            var db = _redisConnectionMultiplexer.GetDatabase();
            foreach (var document in input)
            {
                db.StringSet(document.Id, document.ToString());
                log.LogInformation($"Saved item with id {input.Count} in Azure Redis cache");
            }
        }
    }
}
