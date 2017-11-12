using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slave.Services;
using Slave.Infrastructure.Middlewares;
using Microsoft.Extensions.HealthChecks;

namespace Slave
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
            services.AddHealthChecks(checks =>
            {
                // cache to zero only for demo/testing purposes, do not put zero otherwise
                checks.AddValueTaskCheck("HTTP Endpoint",
                () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")),
                TimeSpan.Zero);
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseFailingMiddleware(options =>
            //{
            //    options.ConfigPath = "/Failing";
            //});

            app.UseMvc();
        }
    }
}
