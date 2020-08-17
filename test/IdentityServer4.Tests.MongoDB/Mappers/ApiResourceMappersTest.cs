using System.Collections.Generic;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.Tests.MongoDB.Mappers
{
    
    public class ApiResourceMappersTest
    {

        [Fact]
        public void ToEntity_UsingStringsAsScopes_MapsCorrectly()
        {
            var toBeMapped = new Models.ApiResource("test-resource", "Test Resource") {
                Scopes = {"test", "test.scope1", "test.scope2"}
            };

            var mappedEntity = toBeMapped.ToEntity();
            
            Assert.Collection(
                mappedEntity.Scopes,
                scope => Assert.Equal("test", scope),
                scope => Assert.Equal("test.scope1", scope),
                scope => Assert.Equal("test.scope2", scope)
            );
        }

        [Fact]
        public void ToModel_UsingStringsAsScopes_MapsCorrectly()
        {
            var toBeMapped = new ApiResource {
                Name = "my-api",
                DisplayName = "My API",
                Scopes = new List<string> { "api", "api.scope1", "api.scope2" }
            };

            var mappedModel = toBeMapped.ToModel();
            
            Assert.Collection(
                mappedModel.Scopes,
                scope => Assert.Equal("api", scope),
                scope => Assert.Equal("api.scope1", scope),
                scope => Assert.Equal("api.scope2", scope)
            );
        }
    }
}