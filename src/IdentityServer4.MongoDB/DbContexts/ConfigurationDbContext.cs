// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.MongoDB.Configuration;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class ConfigurationDbContext : MongoDBContextBase, IConfigurationDbContext
    {
        private readonly IMongoCollection<Client> _clients;
        private readonly IMongoCollection<IdentityResource> _identityResources;
        private readonly IMongoCollection<ApiResource> _apiResources;

        public ConfigurationDbContext(IOptions<MongoDBConfiguration> settings)
            : base(settings)
        {
            _clients = Database.GetCollection<Client>(Constants.TableNames.Client);
            _identityResources = Database.GetCollection<IdentityResource>(Constants.TableNames.IdentityResource);
            _apiResources = Database.GetCollection<ApiResource>(Constants.TableNames.ApiResource);
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var indexOptions = new CreateIndexOptions() { Background = true };
            _clients.Indexes.CreateOne(Builders<Client>.IndexKeys.Ascending(_ => _.ClientId), indexOptions);
            _identityResources.Indexes.CreateOne(Builders<IdentityResource>.IndexKeys.Ascending(_ => _.Name), indexOptions);
            _apiResources.Indexes.CreateOne(Builders<ApiResource>.IndexKeys.Ascending(_ => _.Name), indexOptions);
            _apiResources.Indexes.CreateOne(Builders<ApiResource>.IndexKeys.Ascending(_ => _.Scopes), indexOptions);
        }

        public IQueryable<Client> Clients
        {
            get { return _clients.AsQueryable(); }
        }

        public IQueryable<IdentityResource> IdentityResources
        {
            get { return _identityResources.AsQueryable(); }
        }

        public IQueryable<ApiResource> ApiResources
        {
            get { return _apiResources.AsQueryable(); }
        }

        public async Task AddClient(Client entity)
        {
            await _clients.InsertOneAsync(entity);
        }

        public async Task AddIdentityResource(IdentityResource entity)
        {
            await _identityResources.InsertOneAsync(entity);
        }

        public async Task AddApiResource(ApiResource entity)
        {
            await _apiResources.InsertOneAsync(entity);
        }
    }
}