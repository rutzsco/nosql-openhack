using System;

using Demo.Function.Movies.Api.Data;

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Demo.Function.Movies.API.Startup))]

namespace Demo.Function.Movies.API
{
    public class Startup : FunctionsStartup
    {
        private static readonly IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register the CosmosClient as a Singleton

            builder.Services.AddSingleton((s) => {

                var cs = new CosmosConnectionStringUtility(configuration["CosmosDBConnection"]);
                CosmosClientBuilder configurationBuilder = new CosmosClientBuilder(cs.AccountEndpoint.ToString(), cs.AccountKey);
                return configurationBuilder
                        .Build();
            });
        }
    }
}