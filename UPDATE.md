# Breaking Changes 

## **v4.0.0**

As of IdentityServer v4.0.0 API scopes in the API resource model are now stored as strings. So, it will require a data schema update.

The *Scopes* property of *APIResource* object must change:

From:

```
Scopes =
{
    new Scope()
    {
        Name = "api2.full_access",
        DisplayName = "Full access to API 2"
    },
    new Scope
    {
        Name = "api2.read_only",
        DisplayName = "Read only access to API 2"
    }
}
```

To:

```
Scopes =
{
    "api2.full_access",
    "api2.read_only"
}
```

*ApiScope* objects are stored in an exclusive colletion inside *ConfigurationDbContext*.