using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer4Sample {

    /// <summary>
    /// 客户端模式和密码模式服务端实现
    /// </summary>
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddIdentityServer ()

                //客户端模式（client credentials）
                .AddDeveloperSigningCredential ()
                .AddInMemoryApiResources (Config.GetResources ())
                .AddInMemoryClients (Config.GetClients ())

                //密码模式（resource owner password credentials）新增
                .AddTestUsers (Config.GetTestUsers ());

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            //配置HTTTPS
            //else
            //{
            //app.UseHsts();
            //}

            //app.UseHttpsRedirection();

            //app.UseMvc();//WebAPI 和 MVC使用的是同一个管道

            app.UseIdentityServer ();
        }
    }
}