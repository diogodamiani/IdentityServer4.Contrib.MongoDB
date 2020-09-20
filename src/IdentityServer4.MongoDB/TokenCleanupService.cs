using IdentityServer4.MongoDB.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
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
      if (_options.Interval < 1) throw new ArgumentException("interval must be more than 1 second");
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogDebug("Stopping grant removal");
      return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogDebug("Starting grant removal");
      if (_options.Enable)
      {
        while (!stoppingToken.IsCancellationRequested)
        {
          await RemoveExpiredGrantsAsync().ConfigureAwait(false);

          stoppingToken.WaitHandle.WaitOne(_options.Interval * 1000); // ms
        }
      }
    }

    private async Task RemoveExpiredGrantsAsync()
    {
      try
      {
        using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
          var tokenCleanup = serviceScope.ServiceProvider.GetRequiredService<TokenCleanup>();
          await tokenCleanup.RemoveExpiredGrantsAsync();
        }
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
      }
    }
  }
}
