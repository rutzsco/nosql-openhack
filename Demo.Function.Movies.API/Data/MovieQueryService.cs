
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
        //SELECT top 10 c.ItemId from c order by c.Popularity desc
        public async Task<QueryResult<IEnumerable<Movie>>> GetTopByPopularity()
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Item");
            QueryDefinition query = new QueryDefinition("SELECT top 10 * from c order by c.Popularity desc");
            List<Movie> results = new List<Movie>();
            double requestCharge = 0;
            using (FeedIterator<Movie> resultSetIterator = container.GetItemQueryIterator<Movie>(query))
            {
                while (resultSetIterator.HasMoreResults)
                {
                    Microsoft.Azure.Cosmos.FeedResponse<Movie> response = await resultSetIterator.ReadNextAsync();
                    requestCharge = requestCharge + response.RequestCharge;
                    results.AddRange(response);
                }
            }
            var qr = new QueryResult<IEnumerable<Movie>>(results, requestCharge);
            return qr;
        }

        public async Task<QueryResult<IEnumerable<Movie>>> GetTopByReleaseDate()
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Item");
            QueryDefinition query = new QueryDefinition("SELECT top 10 * from c order by c.ReleaseDate desc");
            List<Movie> results = new List<Movie>();
            double requestCharge = 0;
            using (FeedIterator<Movie> resultSetIterator = container.GetItemQueryIterator<Movie>(query))
            {
                while (resultSetIterator.HasMoreResults)
                {
                    Microsoft.Azure.Cosmos.FeedResponse<Movie> response = await resultSetIterator.ReadNextAsync();
                    requestCharge = requestCharge + response.RequestCharge;
                    results.AddRange(response);
                }
            }
            var qr = new QueryResult<IEnumerable<Movie>>(results, requestCharge);
            return qr;
        }

        public async Task<QueryResult<IEnumerable<Movie>>> GetAll()
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Item");
            QueryDefinition query = new QueryDefinition("SELECT * FROM C");
            List<Movie> results = new List<Movie>();
            double requestCharge = 0;
            using (FeedIterator<Movie> resultSetIterator = container.GetItemQueryIterator<Movie>(query))
            {
                while (resultSetIterator.HasMoreResults)
                {
                    Microsoft.Azure.Cosmos.FeedResponse<Movie> response = await resultSetIterator.ReadNextAsync();
                    requestCharge = requestCharge + response.RequestCharge;
                    results.AddRange(response);
                }
            }
            var qr = new QueryResult<IEnumerable<Movie>>(results, requestCharge);
            return qr;
        }

        public async Task<QueryResult<IEnumerable<Movie>>> GetAllByCategory(string category)
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Item");
            QueryDefinition query = new QueryDefinition("SELECT * FROM C WHERE C.CategoryId = @category"); 
            query.WithParameter("@category", Convert.ToInt32(category));

            List<Movie> results = new List<Movie>();
            double requestCharge = 0;
            using (FeedIterator<Movie> resultSetIterator = container.GetItemQueryIterator<Movie>(query))
            {
                while (resultSetIterator.HasMoreResults)
                {
                    Microsoft.Azure.Cosmos.FeedResponse<Movie> response = await resultSetIterator.ReadNextAsync();
                    requestCharge = requestCharge + response.RequestCharge;
                    results.AddRange(response);
                }
            }
            var qr = new QueryResult<IEnumerable<Movie>>(results, requestCharge);
            return qr;
        }

        public async Task<QueryResult<IEnumerable<Category>>> GetCategories()
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Category");
            QueryDefinition query = new QueryDefinition("SELECT * FROM C");
            List<Category> results = new List<Category>();
            double requestCharge = 0;
            using (FeedIterator<Category> resultSetIterator = container.GetItemQueryIterator<Category>(query))
            {
                while (resultSetIterator.HasMoreResults)
                {
                    Microsoft.Azure.Cosmos.FeedResponse<Category> response = await resultSetIterator.ReadNextAsync();
                    requestCharge = requestCharge + response.RequestCharge;
                    results.AddRange(response);
                }
            }

            var qr = new QueryResult<IEnumerable<Category>>(results, requestCharge);
            return qr;
        }


        public async Task<double> CreateCartItem(string itemId, string cartId)
        {
            var id = Guid.NewGuid().ToString();
            var ci = new 
            { 
                id = id,
                CartItemId = id,
                CartId = cartId,
                ItemId = itemId,
                Quantity = 1,
                DateCreated = DateTime.UtcNow
            };
            var container = this._cosmosClient.GetContainer(DatabaseName, "CartItem");
            var response = await container.CreateItemAsync(ci, new PartitionKey(id));

            return response.RequestCharge;
        }

        public async Task<QueryResult<Movie>> GetById(string id)
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Item");
            var result = await container.ReadItemAsync<Movie>(id, new PartitionKey(Convert.ToInt32(id)));

            var qr = new QueryResult<Movie>(result, result.RequestCharge);
            return qr;
        }
    }
}
