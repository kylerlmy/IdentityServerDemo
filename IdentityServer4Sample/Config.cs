﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer4Sample {
    public class Config {
        public static IEnumerable<ApiResource> GetResources () {
            return new List<ApiResource> {
                new ApiResource ("api", " My Api")
            };
        }

        public static IEnumerable<Client> GetClients () {
            return new List<Client> {
                new Client () {
                    ClientId = "client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = { new Secret ("secret".Sha256 ()) },
                        AllowedScopes = { "api" }
                        },
                        new Client () {
                        ClientId = "pwdClient",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        ClientSecrets = { new Secret ("secret".Sha256 ()) },
                        RequireClientSecret=false,//设置为false后，就不要在请求体中再添加 clent_secret
                        AllowedScopes = { "api" }
                        }
            };
        }

        public static List<TestUser> GetTestUsers () {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "1",
                        Username = "kyle",
                        Password = "123456"
                }
            };
        }

    }
}