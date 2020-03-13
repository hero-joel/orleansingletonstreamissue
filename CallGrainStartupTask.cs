using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Test.Host
{
    public class CallGrainStartupTask : IStartupTask
    {
        private readonly IGrainFactory _grainFactory;

        private readonly ILogger<CallGrainStartupTask> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CallGrainStartupTask(IGrainFactory grainFactory, IServiceScopeFactory serviceScopeFactory, ILogger<CallGrainStartupTask> logger)
        {
            _grainFactory = grainFactory;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
   
        }
    }
}