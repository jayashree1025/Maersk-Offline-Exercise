using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobBackGroundService : BackgroundService
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private readonly ILogger<SortJobBackGroundService> _logger;
        public SortJobBackGroundService(ISortJobProcessor sortJobProcessor,
            ILogger<SortJobBackGroundService> logger)
        {
            _sortJobProcessor = sortJobProcessor;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _sortJobProcessor.ProcessAsync();
            }
        }
    }
}
