using System;
using System.Net.Http;
using IdentityModel.Client;
namespace ThirdPartyPwdClient
{
    /// <summary>
    /// 客户端模式对应控制台的客户端[IdentityServer4Sample作为服务端]
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
           Do();
        }

        private static void Do () {
            var diso =  DiscoveryClient.GetAsync ("http://localhost:5000").Result;
            if (diso.IsError) {
                Console.WriteLine (diso.Error);
            }

            var tokenClient = new TokenClient (diso.TokenEndpoint, "client", "secret");
            var tokenResponse =  tokenClient.RequestClientCredentialsAsync ("api").Result;
            

            if (tokenResponse.IsError) {
                Console.WriteLine (tokenResponse.Error);
            } else {
                Console.WriteLine (tokenResponse.Json);
            }

            var httpClient = new HttpClient ();
            httpClient.SetBearerToken(tokenResponse.AccessToken);//设置Http Head 中 authorization 对应的值
            
            var response=httpClient.GetAsync("http://localhost:5001/api/values").Result;

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }

            Console.ReadLine();

        }
    }
}
