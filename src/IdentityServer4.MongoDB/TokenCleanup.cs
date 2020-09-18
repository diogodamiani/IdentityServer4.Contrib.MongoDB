// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.MongoDB.Interfaces;
using IdentityServer4.MongoDB.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB
{
    public class TokenCleanup
    {
        private readonly IPersistedGrantDbContext _persistedGrantDbContext;
        private readonly ILogger<TokenCleanup> _logger;

        public TokenCleanup(IPersistedGrantDbContext persistedGrantDbContext, ILogger<TokenCleanup> logger)
        {
            _persistedGrantDbContext = persistedGrantDbContext;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveExpiredGrantsAsync() {
            try {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync();
                //await RemoveDeviceCodesAsync();
            }
            catch (Exception ex) {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }

        /// <summary>
        /// Removes the stale persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveGrantsAsync()
        {
            var expired = _persistedGrantDbContext.PersistedGrants
                .Where(x => x.Expiration < DateTime.UtcNow)
                .ToArray();

            var found = expired.Length;
            _logger.LogDebug("Removing {grantCount} tokens", found);

            if (found > 0)
            {
                await _persistedGrantDbContext.RemoveExpired();
            }
        }
    }
}