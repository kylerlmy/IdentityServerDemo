using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace MvcAuth
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {
                new ApiResource ("api1", "Api Application")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client () {
                    ClientId = "mvc",
                        AllowedGrantTypes = GrantTypes.Implicit,//简化模式
                        ClientSecrets = { new Secret ("secret".Sha256 ()) },

                        RequireConsent=false,
                        RedirectUris={ "http://localhost:5001/signin-oidc"},
                        PostLogoutRedirectUris={ "http://localhost:5001/signout-callback-oidc"},
                        //RequireClientSecret=false,//设置为false后，就不要在请求体中再添加 clent_secret
                        //AllowedScopes = { "api1" }
                        AllowedScopes = { IdentityServerConstants.StandardScopes.Profile,IdentityServerConstants.StandardScopes.OpenId }

                        }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "10000",
                        Username = "kyle",
                        Password = "123456",
                        
                        //Claims=new List<Claim>{ new Claim(JwtClaimTypes.Email,"netkylee@gmail.com"),
                        //new Claim(JwtClaimTypes.Role,"user")},
                }
            };
        }

    }
}