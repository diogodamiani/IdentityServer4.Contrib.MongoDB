using MongoDB.Driver;

namespace IdentityServer4.MongoDB.Configuration
{
    public class MongoDBConfiguration
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public SslSettings SslSettings { get; set; }
    }
}
