using System.Collections.Generic;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.Tests.MongoDB.Mappers
{
    public class ApiScopeMappersTest
    {

        [Fact]
        public void ToModel_UserClaims_MapsCorrectly()
        {
            var toBeMapped = new ApiScope {
                Name = "aScope",
                UserClaims = new List<ApiScopeClaim> {
                    new ApiScopeClaim {Id = 1, Type = "claim1"},
                    new ApiScopeClaim {Id = 2, Type = "claim2"}
                }
            };

            var mappedModel = toBeMapped.ToModel();
            
            Assert.Collection(
                mappedModel.UserClaims,
                claim => Assert.Equal("claim1", claim),
                claim => Assert.Equal("claim2", claim)
            );
        }
    }
}