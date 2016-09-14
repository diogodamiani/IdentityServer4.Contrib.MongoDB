// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using System.Reflection;
using Host.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using IdentityServer4.MongoDB.DbContexts;
using IdentityServer4.MongoDB;
using Microsoft.Extensions.Options;
using IdentityServer4.MongoDB.Configuration;
using IdentityServer4.MongoDB.Mappers;
using Microsoft.Extensions.Configuration;
using IdentityServer4.MongoDB.Interfaces;

namespace Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IHostingEnvironment env)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = configurationBuilder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer()
                .SetTemporarySigningCredential()
                .AddInMemoryUsers(Users.Get())
                
                .AddConfigurationStore(_configuration)
                .AddOperationalStore(_configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(@"c:\logs\IdentityServer4.MongoDB.Host.txt")
                .CreateLogger();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            //app.UseDeveloperExceptionPage();
            
            // Setup Databases
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                EnsureSeedData(serviceScope.ServiceProvider.GetService<IConfigurationDbContext>());

                var options = serviceScope.ServiceProvider.GetService<IOptions<MongoDBConfiguration>>();
                var tokenCleanup = new TokenCleanup(options);
                tokenCleanup.Start();
            }

            app.UseIdentityServer();
            
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private static void EnsureSeedData(IConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Clients.Get().ToList())
                {
                    context.AddClient(client.ToEntity());
                }
            }

            if (!context.Scopes.Any())
            {
                foreach (var scope in Scopes.Get().ToList())
                {
                    context.AddScope(scope.ToEntity());
                }
            }
        }
    }
}