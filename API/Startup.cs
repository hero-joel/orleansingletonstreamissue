using System;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Test.Host.Api.HealthChecks;
using Test.Hubs;
using Test.Silo.Services;

namespace Test.Host.Api
{
    /// <summary>
    /// This handles the setup of all endpoints for web and signalr.
    /// This runs on same host as the Orleans host and is dependent on several DI's setup in there.
    /// </summary>
    public class Startup
    {

        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

       
            #region healthcheck
      
            services.AddHealthChecks()
                 .AddCheck<ClusterHealthCheck>("ClusterHealth");

            #endregion

            // Add MVC and custom formatters.
            services
                .AddMvc((options) =>
                {   // if we skip json and do custom format.
                    //    options.InputFormatters.Insert(0, new DovahzulInputFormatterV1());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                }
                )
                .AddControllersAsServices();


            services
                .AddCors(o =>
                {
                    o.AddPolicy("AllowAll", builder =>
                        {
                            builder.WithOrigins("http://localhost")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials()
                                    .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
                        });
                });

            services.AddSignalR()
             //   .AddMessagePackProtocol()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.PayloadSerializerOptions.IgnoreNullValues = true;

                });

            // Add our custom hub lifetimemanager.
            services.AddSingleton(typeof(HubLifetimeManager<>), typeof(CustomHubLifetimeManager<>));

            // To be able to access context and read headers and IP from controller.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
 

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHealthChecks("/health");

            app.UseCors("AllowAll");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PlayHub>("/playhub");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

}