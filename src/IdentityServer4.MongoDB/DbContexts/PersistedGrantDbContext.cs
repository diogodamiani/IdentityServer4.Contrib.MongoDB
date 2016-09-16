// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Interfaces;
using MongoDB.Driver;
using System.Linq;
using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using IdentityServer4.MongoDB.Configuration;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class PersistedGrantDbContext : MongoDBContextBase, IPersistedGrantDbContext
    {
        private IMongoCollection<PersistedGrant> _persistedGrants;

        public PersistedGrantDbContext(IOptions<MongoDBConfiguration> settings) 
            : base(settings)
        {
            _persistedGrants = Database.GetCollection<PersistedGrant>(Constants.TableNames.PersistedGrant);
        }

        public IQueryable<PersistedGrant> PersistedGrants
        {
            get { return _persistedGrants.AsQueryable(); }
        }

        public async Task Update(Expression<Func<PersistedGrant, bool>> filter, PersistedGrant entity)
        {
            await _persistedGrants.ReplaceOneAsync(filter, entity);
        }

        public async Task Add(PersistedGrant entity)
        {
           await _persistedGrants.InsertOneAsync(entity);
        }
        
        public async Task Remove(Expression<Func<PersistedGrant, bool>> filter)
        {
            await _persistedGrants.DeleteManyAsync(filter);
        }

        public async Task RemoveExpired()
        {
           await Remove(x => x.Expiration < DateTime.UtcNow);
        }
    }
}