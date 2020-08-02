// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;

using IdentityServer4.MongoDB.Entities;

using System.Linq;

namespace IdentityServer4.MongoDB.Mappers
{
    /// <summary>
    /// AutoMapper configuration for API resource
    /// Between model and entity
    /// </summary>
    public class ApiResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="ApiResourceMapperProfile"/>
        /// </summary>
        public ApiResourceMapperProfile()
        {
            // entity to model
            CreateMap<ApiResource, Models.ApiResource>(MemberList.Destination)
                .ForMember(x => x.Properties,
                    opt => opt.MapFrom(src => src.Properties.ToDictionary(item => item.Key, item => item.Value)))
                .ForMember(x => x.ApiSecrets, opt => opt.MapFrom(src => src.Secrets.Select(x => x)))
                .ForMember(x => x.Scopes, opt => opt.MapFrom(src => src.Scopes.Select(x => x)))
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => x.Type)));
            CreateMap<ApiSecret, Models.Secret>(MemberList.Destination);
            CreateMap<ApiScope, Models.ApiScope>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            // model to entity
            CreateMap<Models.ApiResource, ApiResource>(MemberList.Source)
                .ForMember(x => x.Properties,
                    opt => opt.MapFrom(src => src.Properties.ToDictionary(item => item.Key, item => item.Value)))
                .ForMember(x => x.Secrets, opts => opts.MapFrom(src => src.ApiSecrets.Select(x => x)))
                .ForMember(x => x.Scopes, opts => opts.MapFrom(src => src.Scopes.Select(x => x)))
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiResourceClaim { Type = x })));
            CreateMap<Models.Secret, ApiSecret>(MemberList.Source);
            CreateMap<Models.ApiScope, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })));
        }
    }
}