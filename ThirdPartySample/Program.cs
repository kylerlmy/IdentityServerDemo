using System;
using System.Net.Http;
using IdentityModel.Client;

namespace ThirdPartySample {
    class Program {
        static void Main (string[] args) {

            Do ();

        }

        private static void Do () {
            var diso =  DiscoveryClient.GetAsync ("http://localhost:5000").Result;
            if (diso.IsError) {
                Console.WriteLine (diso.Error);
            }

            var tokenClient = new TokenClient (diso.TokenEndpoint, "pwdClient", "secret");//如何服务端的Client中的设置RequireClientSecret=false，则这里可以不指定 “secret”这个参数
            var tokenResponse =  tokenClient.RequestResourceOwnerPasswordAsync("kyle","123456").Result;
            

            if (tokenResponse.IsError) {
                Console.WriteLine (tokenResponse.Error);
            } else {
                Console.WriteLine (tokenResponse.Json);
            }

            var httpClient = new HttpClient ();
            httpClient.SetBearerToken(tokenResponse.AccessToken);
            
            var response=httpClient.GetAsync("http://localhost:5001/api/values").Result;

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }

            Console.ReadLine();

        }
    }
}