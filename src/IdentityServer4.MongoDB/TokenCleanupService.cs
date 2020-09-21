using IdentityServer4.MongoDB.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB
{
    /// <summary>
    /// Helper to cleanup expired persisted grants.
    /// </summary>
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TokenCleanupOptions _options;
        private readonly ILogger<TokenCleanupService> _logger;

        public TokenCleanupService(IServiceProvider serviceProvider, TokenCleanupOptions options, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            if (!_options.Enable)
            {
                _logger.LogDebug("Grant removal is not enabled");
                return;
            }

            if (_options.Interval < 1)
            {
                _logger.LogDebug("Grant removal interval must be more than 1 second");
                return;
            }

            try
            {
                _logger.LogDebug("Grant removal started");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(_options.Interval * 1000, stoppingToken); // ms
                    await RemoveExpiredGrantsAsync();
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError("Error running grant removal: {exception}", ex.Message);
            }
            finally
            {
                _logger.LogDebug("Grant removal ended");
            }
        }

        private async Task RemoveExpiredGrantsAsync()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var tokenCleanup = serviceScope.ServiceProvider.GetRequiredService<TokenCleanup>();
                await tokenCleanup.RemoveExpiredGrantsAsync();
            }
        }
    }
}
