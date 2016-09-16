// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Interfaces;
using System.Linq;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using IdentityServer4.MongoDB.Configuration;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class ConfigurationDbContext : MongoDBContextBase, IConfigurationDbContext
    {
        private IMongoCollection<Client> _clients;
        private IMongoCollection<Scope> _scopes;

        public ConfigurationDbContext(IOptions<MongoDBConfiguration> settings)
            : base(settings)
        {
            _clients = Database.GetCollection<Client>(Constants.TableNames.Client);
            _scopes = Database.GetCollection<Scope>(Constants.TableNames.Scope);
        }

        public IQueryable<Client> Clients
        {
            get { return _clients.AsQueryable(); }
        }
        public IQueryable<Scope> Scopes
        {
            get { return _scopes.AsQueryable(); }
        }

        public async Task AddClient(Client entity)
        {
            await _clients.InsertOneAsync(entity);
        }

        public async Task AddScope(Scope entity)
        {
            await _scopes.InsertOneAsync(entity);
        }

    }
}