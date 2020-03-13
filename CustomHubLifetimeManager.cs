using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Test.Common.Constants;
using Test.Contracts;

namespace Test.Silo.Services
{
    //Note, we extend DefaulttimeManager rather than implent our whole own. that way we just override what we need.
    public class CustomHubLifetimeManager<THub> : DefaultHubLifetimeManager<THub> where THub : Hub
    {
        private readonly ILogger _logger;
        private readonly IClusterClient _clusterClient;
        private IStreamProvider _streamProvider;
        private IAsyncStream<ClientMessage> _serverStream;
        private readonly string _hubName = typeof(THub).Name;
        private readonly SemaphoreSlim _streamSetupLock = new SemaphoreSlim(1);
        public CustomHubLifetimeManager(ILogger<DefaultHubLifetimeManager<THub>> logger, IClusterClient clusterClient) : base(logger)
        {
            this._logger = logger;
            this._clusterClient = clusterClient;

            // start the thread but discard what ever comes from it, _ is a c# 7 discard operator.
            _ = EnsureStreamSetup();
        }

        private async Task EnsureStreamSetup()
        {
            if (_streamProvider != null)
                return;

            await _streamSetupLock.WaitAsync();

            if (_streamProvider != null)
                return;
            try
            {
                await SetupStreams();
            }
            finally
            {
                _streamSetupLock.Release();
            }
        }

        private async Task SetupStreams()
        {

            try{

       
            _logger.LogInformation("Initializing:  CustomHubLifetimeManager", _hubName, HostConstants.ALL_STREAM_ID);

            this._streamProvider = this._clusterClient.GetStreamProvider(HostConstants.STREAM_PROVIDER);
            this._serverStream = this._streamProvider.GetStream<ClientMessage>(HostConstants.ALL_STREAM_ID, HostConstants.STREAM_NAMESPACE);

            var subscribeTasks = new List<Task>
            {
                this._serverStream.SubscribeAsync((msg, token) => this.ProcessServerMessage(msg))
            };

            await Task.WhenAll(subscribeTasks);

            _logger.LogInformation("Initialized complete:  CustomHubLifetimeManager {hubName} (serverId: {serverId})", _hubName, HostConstants.ALL_STREAM_ID);
                 }
            catch(Exception e){
                    _logger.LogError(e.ToString());        
                }


        }

        private async Task ProcessServerMessage(ClientMessage message)
        {
            // message to specific connection.
            if (message.ConnectionId != null)
            {
                await this.SendConnectionAsync(message.ConnectionId, "action", new[] { message.Payload });
            }
            else if (message.Group != null)
            {
                await this.SendGroupAsync(message.Group, "action", new[] { message.Payload });
            }
        }
        public async override Task OnConnectedAsync(HubConnectionContext connection)
        {
            try
            {
                await EnsureStreamSetup();
                
                await base.OnConnectedAsync(connection);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in custom OnConnectedAsync " + e);
            }

        }

        public async override Task OnDisconnectedAsync(HubConnectionContext connection)
        {


            await base.OnDisconnectedAsync(connection);
        }

    }
}