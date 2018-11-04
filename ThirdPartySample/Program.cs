using System;
using System.Net.Http;
using IdentityModel.Client;

namespace ThirdPartySample {

    /// <summary>
    ///  密码模式对应控制台客户端
    /// </summary>
    class Program {
        static void Main (string[] args) {

            Do ();

        }

        private static void Do () {
            var diso =  DiscoveryClient.GetAsync ("http://localhost:5000").Result;//用于发现端点的客户端库。根据 IdentityServer 的基础地址，获取元数据中包含的实际的端点地址
            if (diso.IsError) {
                Console.WriteLine (diso.Error);
            }

            var tokenClient = new TokenClient (diso.TokenEndpoint, "pwdClient", "secret");//如何服务端的Client中的设置RequireClientSecret=false，则这里可以不指定 “secret”这个参数
            var tokenResponse =  tokenClient.RequestResourceOwnerPasswordAsync("kyle","123456").Result;//请求令牌

            //与客户端凭证授权相比，资源所有者密码授权有一个很小但很重要的区别。访问令牌现在将包含一个 sub 信息，
            //该信息是用户的唯一标识。sub 信息可以在调用 API 后通过检查内容变量来被查看，并且也将被控制台应用程序显示到屏幕上。

            //api资源收到第一个请求之后，会去id4服务器公钥，然后用公钥验证token是否合法，如果合法进行后面后面的有效性验证。
            //有且只有第一个请求才会去id4服务器请求公钥，后面的请求都会用第一次请求的公钥来验证，这也是jwt去中心化验证的思想。

            if (tokenResponse.IsError) {
                Console.WriteLine (tokenResponse.Error);
            } else {
                Console.WriteLine (tokenResponse.Json);
            }

            var httpClient = new HttpClient ();
            httpClient.SetBearerToken(tokenResponse.AccessToken);
            //var response = await client.GetAsync("http://localhost:5001/identity"); 获取访问令牌，默认情况下访问令牌将包含 scope 身份信息，生命周期（nbf 和 exp），客户端 ID（client_id） 和 发行者名称（iss）。
            var response =httpClient.GetAsync("http://localhost:5001/api/values").Result;

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }

            Console.ReadLine();

        }
    }
}