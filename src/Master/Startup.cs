using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Master.Services;
using Master.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.HealthChecks;
using Master.Resilience;
using Microsoft.AspNetCore.Http;

namespace Master
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
            services.AddSingleton<GuidProvider>();
            services.AddOptions();
            services.Configure<MySettings>(Configuration.GetSection("MasterSettings"));
            services.AddHealthChecks(checks =>
            {
                // Cache to zero only for demo/testing purposes, do not put zero otherwise
                checks.AddUrlCheck($"{Configuration["MasterSettings:SlaveUri"]}/hc", TimeSpan.Zero);
            });
            services.AddMvc();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register Polly Policies Factory
            services.AddSingleton<IPoliciesFactory, PoliciesFactory>();

            // Register ResilientHttpClient
            services.AddSingleton<IHttpClient, ResilientHttpClient>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            if (!string.IsNullOrEmpty(Configuration["PATH_BASE"])) {
                app.UsePathBase(Configuration["PATH_BASE"]);
            }

            app.UseMvc();
        }
    }
}
