using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Policies;

public class NetworkRetryPolicyProvider
{
    private readonly WorkflowOrchestratorSettings _settings;
    private readonly ILogger<NetworkRetryPolicyProvider> _logger;

    public NetworkRetryPolicyProvider(
        IOptions<WorkflowOrchestratorSettings> settings,
        ILogger<NetworkRetryPolicyProvider> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public AsyncRetryPolicy GetNetworkPolicy()
    {
        return Policy
            .Handle<Exception>(ex => IsTransientError(ex))
            .WaitAndRetryAsync(
                _settings.MaxRetryAttempts,
                attempt => CalculateDelay(attempt),
                onRetry: (exception, delay, attempt, context) =>
                {
                    _logger.LogWarning(
                        "Retry attempt {Attempt} for network operation. Delay: {Delay}ms. Error: {Message}",
                        attempt,
                        delay.TotalMilliseconds,
                        exception.Message);
                });
    }

    private bool IsTransientError(Exception ex)
    {
        return ex is TimeoutException 
            || ex is IOException 
            || (ex is AggregateException ae && ae.InnerExceptions.Any(IsTransientError));
    }

    private TimeSpan CalculateDelay(int attempt)
    {
        if (!_settings.UseExponentialBackoff)
            return TimeSpan.FromSeconds(_settings.InitialRetryDelayInSeconds);

        var delay = TimeSpan.FromSeconds(
            _settings.InitialRetryDelayInSeconds * Math.Pow(2, attempt - 1));
        
        return delay > TimeSpan.FromSeconds(_settings.MaxRetryDelayInSeconds)
            ? TimeSpan.FromSeconds(_settings.MaxRetryDelayInSeconds)
            : delay;
    }
}