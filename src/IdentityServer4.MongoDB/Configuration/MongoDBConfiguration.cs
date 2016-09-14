using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.Configuration
{
    public class MongoDBConfiguration
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
