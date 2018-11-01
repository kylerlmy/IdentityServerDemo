using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAuth.Data
{
    public static class WebHostMigrateExtentions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seed)
            where TContext : DbContext
        {
            

            using (var scope=webHost.Services.CreateScope())//通过WebHost获取ServiceProvider
            {
                var service = scope.ServiceProvider;
                var logger = service.GetRequiredService<ILogger<TContext>>();

                var context = service.GetService<TContext>();

                try
                {
                    context.Database.Migrate();
                    seed(context, service);
                    logger.LogError($"执行 {typeof(TContext).Name}的Seed方法成功");

                }
                catch (Exception)
                {

                    logger.LogError($"执行 {typeof(TContext).Name}的Seed方法失败");
                }
                

            }

            return webHost;
        }
    }
}
