using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace FreeCourse.IdentityServer;

public static class Config
{
    public static IEnumerable<ApiResource> ApiResources =>
        [
            new("resource_catalog"){ Scopes = { "catalog_fullpermission" } },
            new("resource_photo_stock"){ Scopes = { "photo_stock_fullpermission" } },
            new("resource_basket"){ Scopes = { "basket_fullpermission" } },
            new("resource_discount"){ Scopes = { "discount_fullpermission" } },
            new(IdentityServerConstants.LocalApi.ScopeName),
        ];

    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.Email(),
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource(){
                Name="roles",
                DisplayName="Roles",
                Description="Kullan�c� rolleri",
                UserClaims=["role"]
            }
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new("catalog_fullpermission", "Catalog API i�in full eri�im"),
            new("photo_stock_fullpermission", "Photo Stock API i�in full eri�im"),
            new("basket_fullpermission", "Basket API i�in full eri�im"),
            new("discount_fullpermission", "Discount API i�in full eri�im"),
            new(IdentityServerConstants.LocalApi.ScopeName),
        ];

    public static IEnumerable<Client> Clients =>
        [
            new(){
                ClientName = "Asp.Net Core MVC",
                ClientId = "WebMvcClient",
                ClientSecrets= { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "catalog_fullpermission", "photo_stock_fullpermission", IdentityServerConstants.LocalApi.ScopeName },
            },
            new(){
                ClientName = "Asp.Net Core MVC",
                ClientId = "WebMvcClientForUser",
                AllowOfflineAccess = true,
                ClientSecrets= { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {"basket_fullpermission", "discount_fullpermission", IdentityServerConstants.StandardScopes.Email, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.OfflineAccess, IdentityServerConstants.LocalApi.ScopeName,"roles"},
                AccessTokenLifetime= 1*60*60,
                RefreshTokenExpiration= TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime= (int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
                RefreshTokenUsage= TokenUsage.ReUse
            }
        ];
}
