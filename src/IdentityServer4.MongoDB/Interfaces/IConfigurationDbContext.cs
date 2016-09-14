// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using IdentityServer4.MongoDB.Entities;
using System.Linq;

namespace IdentityServer4.MongoDB.Interfaces
{
    public interface IConfigurationDbContext //: IDisposable
    {
        IQueryable<Client> Clients { get; }
        IQueryable<Scope> Scopes { get; }

        Task AddClient(Client entity);

        Task AddScope(Scope entity);
    }
}