// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.MongoDB.Configuration;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.DbContexts
{
    public class PersistedGrantDbContext : MongoDBContextBase, IPersistedGrantDbContext
    {
        private readonly IMongoCollection<PersistedGrant> _persistedGrants;

        public PersistedGrantDbContext(IOptions<MongoDBConfiguration> settings)
            : base(settings)
        {
            _persistedGrants = Database.GetCollection<PersistedGrant>(Constants.TableNames.PersistedGrant);
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var indexOptions = new CreateIndexOptions() { Background = true };
            var persistedGrandIndexKeys = Builders<PersistedGrant>.IndexKeys;
            
            _persistedGrants.Indexes.CreateOne(persistedGrandIndexKeys.Ascending(_ => _.Key), indexOptions);

            _persistedGrants.Indexes.CreateOne(persistedGrandIndexKeys.Ascending(_ => _.SubjectId), indexOptions);
            
            _persistedGrants.Indexes.CreateOne(persistedGrandIndexKeys.Combine(
                persistedGrandIndexKeys.Ascending(_ => _.ClientId),
                persistedGrandIndexKeys.Ascending(_ => _.SubjectId)));
            
            _persistedGrants.Indexes.CreateOne(persistedGrandIndexKeys.Combine(
                persistedGrandIndexKeys.Ascending(_ => _.ClientId),
                persistedGrandIndexKeys.Ascending(_ => _.SubjectId),
                persistedGrandIndexKeys.Ascending(_ => _.Type)));
        }

        public IQueryable<PersistedGrant> PersistedGrants
        {
            get { return _persistedGrants.AsQueryable(); }
        }

        public Task Remove(Expression<Func<PersistedGrant, bool>> filter)
        {
            return _persistedGrants.DeleteManyAsync(filter);
        }

        public Task RemoveExpired()
        {
            return Remove(x => x.Expiration < DateTime.UtcNow);
        }

        public Task InsertOrUpdate(Expression<Func<PersistedGrant, bool>> filter, PersistedGrant entity)
        {
            return _persistedGrants.ReplaceOneAsync(filter, entity, new UpdateOptions() {IsUpsert = true});
        }
    }
}