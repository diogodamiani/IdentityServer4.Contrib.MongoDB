// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using IdentityServer4.MongoDB.Interfaces;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.Stores
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IPersistedGrantDbContext _context;
        private readonly ILogger<PersistedGrantStore> _logger;

        public PersistedGrantStore(IPersistedGrantDbContext context, ILogger<PersistedGrantStore> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task StoreAsync(PersistedGrant token)
        {
            try
            {
                _logger.LogDebug("Try to save or update {persistedGrantKey} in database", token.Key);
                await _context.InsertOrUpdate(t => t.Key == token.Key, token.ToEntity());
                _logger.LogDebug("{persistedGrantKey} stored in database", token.Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Exception storing persisted grant");
                throw;
            }
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = _context.PersistedGrants.FirstOrDefault(x => x.Key == key);
            var model = persistedGrant.ToModel();

            _logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, model != null);

            return Task.FromResult(model);
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            Validate(filter);

            var persistedGrants = _context.PersistedGrants.Where(
                x => (string.IsNullOrWhiteSpace(filter.SubjectId) || x.SubjectId == filter.SubjectId) &&
                                  (string.IsNullOrWhiteSpace(filter.ClientId) || x.ClientId == filter.ClientId) &&
                                  (string.IsNullOrWhiteSpace(filter.Type) || x.Type == filter.Type)).ToList();

            var model = persistedGrants.Select(x => x.ToModel());

            _logger.LogDebug($"{persistedGrants.Count} persisted grants found for filter: {filter}");

            return Task.FromResult(model);
        }

        public Task RemoveAsync(string key)
        {
            _logger.LogDebug("removing {persistedGrantKey} persisted grant from database", key);

            _context.Remove(x => x.Key == key);
            
            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            Validate(filter);
            
            _logger.LogDebug($"removing persisted grants from database for filter: {filter}");

            _context.Remove(
                x => (string.IsNullOrWhiteSpace(filter.SubjectId) || x.SubjectId == filter.SubjectId) &&
                     (string.IsNullOrWhiteSpace(filter.ClientId) || x.ClientId == filter.ClientId) &&
                     (string.IsNullOrWhiteSpace(filter.Type) || x.Type == filter.Type));

            return Task.FromResult(0);
        }

        private void Validate(PersistedGrantFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            if (string.IsNullOrWhiteSpace(filter.ClientId) &&
                string.IsNullOrWhiteSpace(filter.SubjectId) &&
                string.IsNullOrWhiteSpace(filter.Type))
            {
                throw new ArgumentException("No filter values set.", nameof(filter));
            }
        }
    }
}