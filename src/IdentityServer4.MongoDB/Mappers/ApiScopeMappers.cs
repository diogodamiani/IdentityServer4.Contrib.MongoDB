using AutoMapper;
using IdentityServer4.MongoDB.Entities;

namespace IdentityServer4.MongoDB.Mappers
{
    public static class ApiScopeMappers
    {
        static ApiScopeMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiScopeMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static Models.ApiScope ToModel(this ApiScope scope)
        {
            return scope == null ? null : Mapper.Map<Models.ApiScope>(scope);
        }

        public static ApiScope ToEntity(this Models.ApiScope scope)
        {
            return scope == null ? null : Mapper.Map<ApiScope>(scope);
        }

    }
}
