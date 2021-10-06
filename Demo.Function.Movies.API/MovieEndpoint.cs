using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Demo.Function.Movies.Api.Data;

namespace Demo.Function.Movies.API
{

    public class MovieEndpoint
    {
        private CosmosClient _cosmosClient;

        public MovieEndpoint(CosmosClient cosmosClient)
        {
            this._cosmosClient = cosmosClient;
        }

        [FunctionName("MovieGetTopEndpoint")]
        public async Task<IActionResult> GetTop([HttpTrigger(AuthorizationLevel.Function, "get", Route = "recommendations/top")] HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var queryService = new MovieQueryService(_cosmosClient);
            var listing = await queryService.GetAll();

            return new OkObjectResult(listing);
        }

        [FunctionName("MovieGetAllEndpoint")]
        public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "movies")] HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var queryService = new MovieQueryService(_cosmosClient);
            var listing = await queryService.GetAll();

            return new OkObjectResult(listing);
        }

        [FunctionName("MovieGetCategoriesEndpoint")]
        public async Task<IActionResult> GetCategories([HttpTrigger(AuthorizationLevel.Function, "get", Route = "categories")] HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var queryService = new MovieQueryService(_cosmosClient);
            var listing = await queryService.GetCategories();

            return new OkObjectResult(listing);
        }

        [FunctionName("MovieGetAllByCategoryEndpoint")]
        public async Task<IActionResult> GetAllByCategory([HttpTrigger(AuthorizationLevel.Function, "get", Route = "moviesbycategory/{id}")] HttpRequest req, string id, ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var queryService = new MovieQueryService(_cosmosClient);
            var listing = await queryService.GetAllByCategory(id);

            return new OkObjectResult(listing);
        }

        [FunctionName("MovieGetEndpoint")]
        public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "movies/{id}")] HttpRequest req, string id, ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var queryService = new MovieQueryService(_cosmosClient);
            var listing = await queryService.GetById(id);

            return new OkObjectResult(listing);
        }
    }
}
