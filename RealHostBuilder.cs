using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Test.Host.Api;
using Test.Common.Constants;

namespace Test.Host
{
    public static class RealHostBuilder
    {

        public static IHostBuilder GetHost(string[] args, IConfigurationRoot config, ILogger hostLogger)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (hostLogger == null)
                throw new ArgumentNullException(nameof(hostLogger));

            hostLogger.Information($"--------- Building Host ---------");

 


            // Need this for it to work with Docker. 
            var name = Dns.GetHostName(); // get container id
            var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);


            return new HostBuilder()
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp.SetBasePath(Directory.GetCurrentDirectory());
                configApp.AddJsonFile("appsettings.json", optional: true);
                configApp.AddJsonFile(
                    $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                    optional: true);
                configApp.AddEnvironmentVariables();
                configApp.AddCommandLine(args);
            })
            .ConfigureServices(services =>
                {

                    services.AddMemoryCache();

                    services.AddHttpClient();

                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });


                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseOrleans(builder =>
                {
                    builder
                           
                            .Configure<SiloOptions>(options =>
                            {
                                options.SiloName =  Dns.GetHostName();
                            })
                        
                            .Configure<ClusterOptions>(options =>
                            {

                                options.ClusterId = "MyClusterId";
                                options.ServiceId = "MyCluster";
                            })
                          //  .ConfigureEndpoints(siloPort: hostOptions.OrleansSiloPort, gatewayPort: hostOptions.OrleansGatewayPort)
                            .Configure<EndpointOptions>(options =>
                            {
                                options.AdvertisedIPAddress = ip;

                            })
                            .UseLocalhostClustering()
                            .UseInMemoryReminderService()
                            .AddSimpleMessageStreamProvider(HostConstants.STREAM_PROVIDER, opt => opt.FireAndForgetDelivery = true)
                            .AddMemoryGrainStorage("PubSubStore")

                            .AddStartupTask<CallGrainStartupTask>();


                 
                        hostLogger.Information("Adding Orleans dashboard");
                        builder.UseDashboard(options => { });
                    

                });
        }

    }
}