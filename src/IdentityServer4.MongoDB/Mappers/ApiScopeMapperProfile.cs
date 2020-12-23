using AutoMapper;
using IdentityServer4.MongoDB.Entities;
using System.Linq;

namespace IdentityServer4.MongoDB.Mappers
{
    public class ApiScopeMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="ApiScopeMapperProfile"/>
        /// </summary>
        public ApiScopeMapperProfile()
        {
            CreateMap<UserClaim, string>().ConvertUsing(uc => uc.Type);
            
            // entity to model
            CreateMap<ApiScope, Models.ApiScope>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims));

            // model to entity
            CreateMap<Models.ApiScope, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })));
        }
    }
}
