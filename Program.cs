using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Test.Host
{
    public static class Program
    {

        public static Task Main(string[] args)
        {

            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false)
                 .AddEnvironmentVariables()
                 .Build();


            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
                 .Enrich.FromLogContext() 
                 .WriteTo.Console(outputTemplate:
                       "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                     .ReadFrom.Configuration(config)
                     .CreateLogger();
       
             Log.Logger.Warning($"--------- Server Starting ---------");

      
    
            var host = RealHostBuilder.GetHost(args, config,  Log.Logger).UseSerilog();
            return host.RunConsoleAsync();

            


        }


    }
}
