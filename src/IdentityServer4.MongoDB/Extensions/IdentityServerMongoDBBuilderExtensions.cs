// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.MongoDB;
using IdentityServer4.MongoDB.Configuration;
using IdentityServer4.MongoDB.DbContexts;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Interfaces;
using IdentityServer4.MongoDB.Options;
using IdentityServer4.MongoDB.Services;
using IdentityServer4.MongoDB.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class IdentityServerMongoDBBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
           this IIdentityServerBuilder builder, Action<MongoDBConfiguration> setupAction)
        {
            builder.Services.Configure(setupAction);

            return builder.AddConfigurationStore();
        }

        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder, IConfiguration configuration)
        {
            builder.Services.Configure<MongoDBConfiguration>(configuration);

            return builder.AddConfigurationStore();
        }

        public static IIdentityServerBuilder AddOperationalStore(
           this IIdentityServerBuilder builder,
           Action<MongoDBConfiguration> setupAction,
           Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
            builder.Services.Configure(setupAction);

            return builder.AddOperationalStore(tokenCleanUpOptions);
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            IConfiguration configuration,
            Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
            builder.Services.Configure<MongoDBConfiguration>(configuration);

            return builder.AddOperationalStore(tokenCleanUpOptions);
        }

        private static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder)
        {
            ConfigureIgnoreExtraElementsConfigurationStore();

            builder.Services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();

            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            return builder;
        }

        private static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            Action<TokenCleanupOptions> tokenCleanUpOptions = null)
        {
            ConfigureIgnoreExtraElementsOperationalStore();

            builder.Services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>();

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var tco = new TokenCleanupOptions();
            tokenCleanUpOptions?.Invoke(tco);
            builder.Services.AddSingleton(tco);
            builder.Services.AddTransient<TokenCleanup>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupService>();

            return builder;
        }

        private static void ConfigureIgnoreExtraElementsConfigurationStore()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiScope>(cm =>
            {
              cm.AutoMap();
              cm.SetIgnoreExtraElements(true);
            });
        }

        private static void ConfigureIgnoreExtraElementsOperationalStore()
        {
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}