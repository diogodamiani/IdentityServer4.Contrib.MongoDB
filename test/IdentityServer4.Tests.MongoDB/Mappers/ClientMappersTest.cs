using System.Collections.Generic;
using IdentityServer4.MongoDB.Entities;
using IdentityServer4.MongoDB.Mappers;
using Xunit;

namespace IdentityServer4.Tests.MongoDB.Mappers
{
    
    public class ClientMappersTest
    {

        [Fact]
        public void ToModel_ClientClaims_MapsCorrectly()
        {
            var toBeMapped = new Client {
                ClientId = "someId",
                AllowedScopes = new List<ClientScope> { new ClientScope { Scope = "dsquad:public"} },
                ClientSecrets = new List<ClientSecret> { new ClientSecret { Value = "secret" } },
                AllowedGrantTypes = new List<ClientGrantType> { new ClientGrantType {GrantType = "client_credentials"} },
                AccessTokenType = 0,
                AccessTokenLifetime = int.MaxValue,
                Enabled = true,
                AlwaysSendClientClaims = true,
                ClientClaimsPrefix = "",
                Claims = new List<ClientClaim> { new ClientClaim { Type = "hello", Value = "world" } }
            };

            var mappedClient = toBeMapped.ToModel();

            var expectedClientClaim = new Models.ClientClaim {
                Type = "hello",
                Value = "world"
            };
            
            Assert.Collection(
                mappedClient.Claims,
                claim => Assert.Equal(expectedClientClaim, claim)
            );
        }
    }
}