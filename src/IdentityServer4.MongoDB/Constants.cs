// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServer4.MongoDB
{
    public class Constants
    {
        public class TableNames
        {
            // Configuration
            public const string IdentityResource = "IdentityResources";
            public const string IdentityClaim = "IdentityClaims";

            public const string ApiResource = "ApiResources";
            public const string ApiSecret = "ApiSecrets";
            public const string ApiScope = "ApiScopes";
            public const string ApiClaim = "ApiClaims";
            public const string ApiScopeClaim = "ApiScopeClaims";
            public const string User = "Users";

            public const string Client = "Clients";
            public const string ClientGrantType = "ClientGrantTypes";
            public const string ClientRedirectUri = "ClientRedirectUris";
            public const string ClientPostLogoutRedirectUri = "ClientPostLogoutRedirectUris";
            public const string ClientScopes = "ClientScopes";
            public const string ClientSecret = "ClientSecrets";
            public const string ClientClaim = "ClientClaims";
            public const string ClientIdPRestriction = "ClientIdPRestrictions";
            public const string ClientCorsOrigin = "ClientCorsOrigins";

            // Operational
            public const string PersistedGrant = "PersistedGrants";
        }
    }
}