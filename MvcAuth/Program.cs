using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcAuth.Data;

namespace MvcAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                //.MigrateDbContext<ApplicationDbContext>((context,serviceProvider)=> { //在启动之前应用Seed方法
                //    new ApplicationDbContextSeed().SeedAsync(context, serviceProvider)
                //    .Wait();
                //})
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseEnvironment("Development")
              //.UseContentRoot(Directory.GetCurrentDirectory())//把项目的当前目录设置为 Content root，这样项目的 web root 就可以在开发阶段被明确
              .UseUrls("http://localhost:5000")
                .UseStartup<Startup>()
                .Build();
    }
}
