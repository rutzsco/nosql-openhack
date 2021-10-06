
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
            QueryRequestOptions options = new QueryRequestOptions() { MaxItemCount = 10 };
            using (FeedIterator<Movie> resultSetIterator = container.GetItemQueryIterator<Movie>(query, requestOptions: options))
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

        public async Task<double> CreateOrder()
        {
            //var od1 = new OrderDetail("6851", "397520", "13.13", "1", "ezlzih41@esfhmo.net");
           // var od2 = new OrderDetail("6861", "397521", "13.13", "1", "ezlzih41@esfhmo.net");
            var odList = new List<OrderDetail>();
            odList.Add(null);
            odList.Add(null);

            var id = Guid.NewGuid().ToString();
            var o = new
            {
                id = "471",
                OrderId = 471,
                OrderDate = "2018-10-09T00:28:22.750",
    FirstName = "Ben",
    LastName = "Reed",
    Address = "63 White Oak St.",
    City = "Stockton",
    State = "California",
   PostalCode = "29151",
    Country = "United States",
    Phone = "430871-1517",
    Total = 89.41,
    SMSOptIn = true,
    PaymentTransactionId = "DIG84594",
    HasBeenShipped = false,
    OrderDetails = odList
            };

            var container = this._cosmosClient.GetContainer(DatabaseName, "Orders");
            var response = await container.CreateItemAsync(o, new PartitionKey(o.OrderId));

            return response.RequestCharge;
        }

        public async Task<QueryResult<Movie>> GetById(string id)
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Item");
            var result = await container.ReadItemAsync<Movie>(id, new PartitionKey(Convert.ToInt32(id)));

            var qr = new QueryResult<Movie>(result, result.RequestCharge);
            return qr;
        }

        public async Task<QueryResult<Order>> OrderById(string id)
        {
            var container = this._cosmosClient.GetContainer(DatabaseName, "Orders");
            var result = await container.ReadItemAsync<Order>(id, new PartitionKey(Convert.ToInt32(id)));

            var qr = new QueryResult<Order>(result, result.RequestCharge);
            return qr;
        }
    }
}
