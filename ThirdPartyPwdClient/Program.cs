using System;
using System.Net.Http;
using IdentityModel.Client;
namespace ThirdPartyPwdClient
{
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
