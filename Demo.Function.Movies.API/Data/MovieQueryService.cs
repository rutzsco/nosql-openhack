
using Demo.Function.Movies.API.Data;

using Microsoft.Azure.Cosmos;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Function.Movies.Api.Data
{
    public class MovieQueryService
    {
        private CosmosClient _cosmosClient;
        private static readonly string DatabaseName = "MoviesDB";

        public MovieQueryService(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<IEnumerable<Movie>> GetAll()
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Item");
            QueryDefinition query = new QueryDefinition("SELECT * FROM C");
            List<Movie> results = new List<Movie>();
            using (FeedIterator<Movie> resultSetIterator = container.GetItemQueryIterator<Movie>(query))
            {
                while (resultSetIterator.HasMoreResults)
                {
                    Microsoft.Azure.Cosmos.FeedResponse<Movie> response = await resultSetIterator.ReadNextAsync();
                    results.AddRange(response);
                }
            }
            return results;
        }
        public async Task<Movie> GetById(string id)
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Item");
            var result = await container.ReadItemAsync<Movie>(id, new PartitionKey(id));
            return result;
        }
    }
}
