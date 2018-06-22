using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trumgu_IntegratedManageSystem.Filters;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Utils;

namespace Trumgu_IntegratedManageSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(opt =>
            {
                opt.ValueLengthLimit = int.MaxValue;
                opt.MultipartBodyLengthLimit = int.MaxValue;
                opt.BufferBodyLengthLimit = long.MaxValue;
                opt.ValueCountLimit = int.MaxValue;
            });
            services.AddMvc(cfg =>
            {
                cfg.Filters.Add(new ActionFilter());
            });
            services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromSeconds(1800);
            });
            // services.Configure<IISOptions>(options =>
            // {
            //     options.ForwardClientCertificate = false;
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Error/ErrorJump/{0}");
            }
            else
            {
                app.UseExceptionHandler("/Error/ErrorJump/{0}");
            }

            app.UseStatusCodePagesWithReExecute("/Error/ErrorJump/{0}");

            app.UseStaticFiles();
            // 启用Session

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Jump}/{id?}");
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true    // 不限制下载文件的content-type
            });

            // 初始化配置
            var config = new ConfigurationBuilder()
                 .AddInMemoryCollection()    //将配置文件的数据加载到内存中
                 .SetBasePath(Directory.GetCurrentDirectory())   //指定配置文件所在的目录
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
            ConfigConstantHelper.trumgu_ims_db_connstr = config["ConnectionStr:trumgu_ims_db"];
            ConfigConstantHelper.trumgu_bi_db_connstr = config["ConnectionStr:trumgu_bi_db"];
            ConfigConstantHelper.fund_connstr = config["ConnectionStr:fund"];
            ConfigConstantHelper.ProgramName = config["ProgramStr:ProgramName"];
            ConfigConstantHelper.TechnicalSupport = config["ProgramStr:TechnicalSupport"];
        }
    }
}