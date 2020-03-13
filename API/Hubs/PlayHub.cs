


using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;


namespace Test.Hubs
{

    public class PlayHub : Hub
    {
        private readonly IClusterClient _clusterClient;

        private readonly ILogger<PlayHub> _logger;

        public PlayHub(IClusterClient clusterClient, ILogger<PlayHub> logger)
        {
            _clusterClient = clusterClient;
            _logger = logger;
        }

        public async Task echo(string text)
        {
            
            await Clients.Caller.SendAsync("echo", text);
                
        }

    }
}
