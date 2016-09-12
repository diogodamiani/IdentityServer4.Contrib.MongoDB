// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Interfaces;
using System.Linq;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class ConfigurationDbContext : MongoDBContext, IConfigurationDbContext
    {
        private IMongoCollection<Client> _clients;
        private IMongoCollection<Scope> _scopes;

        public ConfigurationDbContext()
        {
            _clients = DB.GetCollection<Client>(Constants.TableNames.Client);
            _scopes = DB.GetCollection<Scope>(Constants.TableNames.Scope);
        }

        public IQueryable<Client> Clients
        {
            get { return _clients.AsQueryable(); }
        }
        public IQueryable<Scope> Scopes
        {
            get { return _scopes.AsQueryable(); }
        }
    }
}