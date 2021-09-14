using System.Collections.Generic;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.Tests.MongoDB.Mappers
{
    public class IdentityResourceMappersTest
    {
        
        [Fact]
        public void ToModel_UserClaimsAreMappedCorrectly()
        {
            var toBeMapped = new IdentityResource {
                Name = "profile",
                UserClaims = new List<IdentityClaim> {
                    new IdentityClaim {Id = 1, Type = "name"},
                    new IdentityClaim {Id = 2, Type = "gender"}
                }
            };

            var mappedModel = toBeMapped.ToModel();
            
            Assert.Collection(
                mappedModel.UserClaims,
                scope => Assert.Equal("name", scope),
                scope => Assert.Equal("gender", scope)
            );
        }
    }
}