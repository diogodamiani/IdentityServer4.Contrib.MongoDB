// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using IdentityServer4.MongoDB.Entities;
using System.Linq;

namespace IdentityServer4.MongoDB.Mappers
{
    /// <summary>
    /// AutoMapper configuration for identity resource
    /// Between model and entity
    /// </summary>
    public class IdentityResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="IdentityResourceMapperProfile"/>
        /// </summary>
        public IdentityResourceMapperProfile()
        {
            CreateMap<UserClaim, string>().ConvertUsing(uc => uc.Type);
            
            // entity to model
            CreateMap<IdentityResource, Models.IdentityResource>(MemberList.Destination)
                .ForMember(x => x.Properties,
                    opt => opt.MapFrom(src => src.Properties.ToDictionary(item => item.Key, item => item.Value)))
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims));

            // model to entity
            CreateMap<Models.IdentityResource, IdentityResource>(MemberList.Source)
                .ForMember(x => x.Properties,
                    opt => opt.MapFrom(src => src.Properties.ToDictionary(item => item.Key, item => item.Value)))
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new IdentityClaim { Type = x })));
        }
    }
}