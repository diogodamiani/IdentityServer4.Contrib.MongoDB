using IdentityServer4.MongoDB.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class MongoDBContextBase : IDisposable
    {
        private readonly IMongoClient _client;

        public MongoDBContextBase(IOptions<MongoDBConfiguration> settings)
        {
            if (settings.Value == null)
                throw new ArgumentNullException(nameof(settings), "MongoDBConfiguration cannot be null.");

            if (settings.Value.ConnectionString == null)
                throw new ArgumentNullException(nameof(settings), "MongoDBConfiguration.ConnectionString cannot be null.");

            var mongoUrl = MongoUrl.Create(settings.Value.ConnectionString);

            if (settings.Value.Database == null && mongoUrl.DatabaseName == null)
                throw new ArgumentNullException(nameof(settings), "MongoDBConfiguration.Database cannot be null.");

            var clientSettings = MongoClientSettings.FromUrl(mongoUrl);

            if (clientSettings.SslSettings != null)
            {
                clientSettings.SslSettings = settings.Value.SslSettings;
                clientSettings.UseSsl = true;
            }
            else
            {
                clientSettings.UseSsl = false;
            }

            _client = new MongoClient(clientSettings);
            Database = _client.GetDatabase(settings.Value.Database ?? mongoUrl.DatabaseName);
        }

        protected IMongoDatabase Database { get; }

        public void Dispose()
        {
            // TODO
        }
    }
}
