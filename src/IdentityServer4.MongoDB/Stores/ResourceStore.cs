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
    public class ResourceStore : IResourceStore
    {
        private readonly IConfigurationDbContext _context;
        private readonly ILogger<ResourceStore> _logger;

        public ResourceStore(IConfigurationDbContext context, ILogger<ResourceStore> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var names = apiResourceNames.ToArray();
            var apis = _context.ApiResources.Where(x => names.Contains(x.Name));

            var results = apis.ToArray();
            var models = results.Select(x => x.ToModel()).ToArray();

            _logger.LogDebug("Found {scopes} API resources in database", models.Select(x => x.Scopes));

            return Task.FromResult(models.AsEnumerable());
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();
            var models = _context.ApiScopes
                .Where(x => names.Contains(x.Name))
                .ToArray()
                .Select(x => x.ToModel())
                .ToArray();

            _logger.LogDebug("Found {scopes} API scopes in database", models.Select(x => x.Name));

            return Task.FromResult(models.AsEnumerable());

        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();

            var apis = _context.ApiResources.Where(x => x.Scopes.Any(y => names.Contains(y)));

            var results = apis.ToArray();
            var models = results.Select(x => x.ToModel()).ToArray();

            _logger.LogDebug("Found {scopes} API resources in database", models.Select(x => x.Scopes));

            return Task.FromResult(models.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var resources = _context.IdentityResources.Where(x => scopes.Contains(x.Name));

            var results = resources.ToArray();

            _logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            return Task.FromResult(results.Select(x => x.ToModel()).ToArray().AsEnumerable());
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var identity = _context.IdentityResources;

            var apis = _context.ApiResources;

            var scopes = _context.ApiScopes;

            var result = new Resources(
                identity.ToArray().Select(x => x.ToModel()).AsEnumerable(),
                apis.ToArray().Select(x => x.ToModel()).AsEnumerable(),
                scopes.ToArray().Select(x => x.ToModel()).AsEnumerable());

            _logger.LogDebug("Found {scopes} as all scopes in database", result.IdentityResources.Select(x => x.Name).Union(result.ApiResources.SelectMany(x => x.Scopes)));

            return Task.FromResult(result);
        }
    }
}