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
        private readonly IMongoCollection<ApiScope> _apiScopes;

        public ConfigurationDbContext(IOptions<MongoDBConfiguration> settings)
            : base(settings)
        {
            _clients = Database.GetCollection<Client>(Constants.TableNames.Client);
            _identityResources = Database.GetCollection<IdentityResource>(Constants.TableNames.IdentityResource);
            _apiResources = Database.GetCollection<ApiResource>(Constants.TableNames.ApiResource);
            _apiScopes = Database.GetCollection<ApiScope>(Constants.TableNames.ApiScope);

            CreateClientsIndexes();
            CreateIdentityResourcesIndexes();
            CreateApiResourcesIndexes();
            CreateApiScopesIndexes();
        }

        private void CreateClientsIndexes()
        {
            var indexOptions = new CreateIndexOptions() { Background = true };

            var builder = Builders<Client>.IndexKeys;
            var clientIdIndexModel = new CreateIndexModel<Client>(builder.Ascending(_ => _.ClientId), indexOptions);
            _clients.Indexes.CreateOne(clientIdIndexModel);
        }

        private void CreateIdentityResourcesIndexes()
        {
            var indexOptions = new CreateIndexOptions() { Background = true };

            var builder = Builders<IdentityResource>.IndexKeys;
            var nameIndexModel = new CreateIndexModel<IdentityResource>(builder.Ascending(_ => _.Name), indexOptions);
            _identityResources.Indexes.CreateOne(nameIndexModel);
        }

        private void CreateApiResourcesIndexes()
        {
            var indexOptions = new CreateIndexOptions() { Background = true };

            var builder = Builders<ApiResource>.IndexKeys;
            var nameIndexModel = new CreateIndexModel<ApiResource>(builder.Ascending(_ => _.Name), indexOptions);
            var scopesIndexModel = new CreateIndexModel<ApiResource>(builder.Ascending(_ => _.Scopes), indexOptions);
            _apiResources.Indexes.CreateOne(nameIndexModel);
            _apiResources.Indexes.CreateOne(scopesIndexModel);
        }

        private void CreateApiScopesIndexes()
        {
            var indexOptions = new CreateIndexOptions { Background = true };

            var builder = Builders<ApiScope>.IndexKeys;
            var nameIndexModel = new CreateIndexModel<ApiScope>(builder.Ascending(_ => _.Name), indexOptions);
            _apiScopes.Indexes.CreateOne(nameIndexModel);
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

        public IQueryable<ApiScope> ApiScopes => this._apiScopes.AsQueryable();

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

        public async Task AddApiScope(ApiScope entity)
        {
            await this._apiScopes.InsertOneAsync(entity);
        }
    }
}