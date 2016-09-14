using IdentityServer4.MongoDB.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class MongoDBContextBase
    {
        private readonly IMongoDatabase _database;
        
        public MongoDBContextBase(IOptions<MongoDBConfiguration> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
        }

        protected IMongoDatabase Database { get { return _database; } }

    }
}
