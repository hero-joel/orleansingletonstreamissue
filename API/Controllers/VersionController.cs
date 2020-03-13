using System;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Test.API.Controllers
{
    [Route("[controller]")]
    public class VersionController : Controller
    {


        public VersionController( )
        {
       
        }


        [HttpGet]
        public string Get()
        {
             
              return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion +"  Core Version:"+ System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
      }


    private static DateTime GetBuildDate(Assembly assembly)
    {
        const string BuildVersionMetadataPrefix = "+build";

        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute?.InformationalVersion != null)
        {
            var value = attribute.InformationalVersion;
            var index = value.IndexOf(BuildVersionMetadataPrefix);
            if (index > 0)
            {
                value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }
        }

        return default;
    }
    }

}
