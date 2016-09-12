using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _db;

        public MongoDBContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _db = client.GetDatabase("db");
        }

        protected IMongoDatabase DB { get { return _db; } }

    }
}
