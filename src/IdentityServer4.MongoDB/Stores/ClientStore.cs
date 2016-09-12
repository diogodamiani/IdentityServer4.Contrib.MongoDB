// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.MongoDB.Interfaces;
using IdentityServer4.MongoDB.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.MongoDB.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly IConfigurationDbContext context;

        public ClientStore(IConfigurationDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            this.context = context;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = context.Clients.FirstOrDefault(x => x.ClientId == clientId);

            var model = client.ToModel();

            return Task.FromResult(model);
        }
    }
}