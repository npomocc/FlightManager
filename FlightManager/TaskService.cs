using System;
using System.Threading;
using System.Threading.Tasks;
using Flight.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlightManager
{
    /// <summary>
    /// Фоновые задачи
    /// </summary>
    internal class TaskService : BackgroundService
    {
        private const string ThisName = nameof(TaskService);
        private readonly IConfiguration _сonfiguration;
        private readonly IServiceProvider _provider;
        public TaskService(IServiceProvider provider,
            IConfiguration configuration)
        {
            _provider = provider;
            _сonfiguration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _provider.CreateScope();
            var flightService = scope
                .ServiceProvider
                .GetRequiredService<IFlightService>();
            var cacheService = scope
                .ServiceProvider
                .GetRequiredService<IApplicationCache>();
            while (!stoppingToken.IsCancellationRequested)
            {
                await cacheService.Remove("flights", stoppingToken);
                _ = await flightService
                    .List(stoppingToken);
                Thread.Sleep(60 * 1000);
            }
        }
    }
}
