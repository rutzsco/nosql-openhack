using Demo.Function.Movies.Api.Data;

using Microsoft.Azure.Documents.Client;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Function.Movies.Api.Data
{
    public class CosmosConnectionStringUtility
    {
        private readonly Uri _serviceEndpoint;
        private readonly string _authKey;

        public CosmosConnectionStringUtility(string connectionString)
        {
            var builder = new DbConnectionStringBuilder() { ConnectionString = connectionString };

            if (builder.TryGetValue("AccountKey", out object key))
            {
                _authKey = key.ToString();
            }

            if (builder.TryGetValue("AccountEndpoint", out object uri))
            {
                _serviceEndpoint = new Uri(uri.ToString());
            }
        }

        public Uri AccountEndpoint { get { return _serviceEndpoint; } }

        public string AccountKey { get { return _authKey; } }
    }
}
