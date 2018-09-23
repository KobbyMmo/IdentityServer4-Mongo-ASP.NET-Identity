using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerSample
{
    /// <summary>
    /// Create Sample / dummy resources, clients and users to enable test
    /// </summary>
    public class Config
    {
        private static string apiScope = "api.sample";

        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                 new IdentityResources.Profile(),
                new IdentityResources.OpenId(),
                 new IdentityResources.Email(),
            };
        }

        /// <summary>
        /// List of Sample APIs
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                 new ApiResource(apiScope, "My Sample API")
                {
                   UserClaims =
                    {
                       JwtClaimTypes.Profile,
                       JwtClaimTypes.Name,
                       JwtClaimTypes.Email,
                    }
                }
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Local",
                    ClientName = "Local",
                    AllowedCorsOrigins = new List<string> { "http://localhost:4200" },
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime=86400,
                    RequireConsent = false,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    RedirectUris = LocalRedirectUris(),
                    PostLogoutRedirectUris = LocalRedirectUris(),
                    AllowedScopes = AllowedScopes(),
                    AllowOfflineAccess = true
                }
            
            };
        }

        public static List<TestUser> GetSampleUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = Guid.NewGuid().ToString(),
                    Username = "admin",
                    Password = "RockStar.1",


                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Name, "Admin "),
                        new Claim(JwtClaimTypes.GivenName, "Admin"),
                        new Claim(JwtClaimTypes.FamilyName, "add min"),
                        new Claim(JwtClaimTypes.Email, "admin@local.com"),
                    }
                }
            };
        }

        private static ICollection<string> StandardScopes()
        {
            return new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                "api.sample"
            };
        }

        private static ICollection<string> LocalRedirectUris()
        {
            return new List<string>
            {
                 "http://localhost:4200/oauth",
                 "http://localhost:4200/oauth/",
                 "http://localhost:4200/redirect",
                 "http://localhost:4200/redirect/",
                 "http://localhost:4200/store",
                 "http://localhost:4200/store/",
                 "http://localhost:4200/login",
                 "http://localhost:4200/login/",
                 "http://localhost:4200/index.html",
                 "http://localhost:4200/index.html/"
            };
        }

        private static ICollection<string> AllowedScopes()
        {
            return new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                apiScope
            };
        }
    }
}
