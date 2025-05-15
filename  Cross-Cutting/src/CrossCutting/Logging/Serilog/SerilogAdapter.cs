using Serilog;
using TheSSS.DicomViewer.Common.Logging;

namespace TheSSS.DicomViewer.Common.Logging.Serilog;

public class SerilogAdapter : ILoggerAdapter
{
    private readonly ILogger _serilogLogger;

    public SerilogAdapter(ILogger serilogLogger)
    {
        _serilogLogger = serilogLogger;
    }

    public void LogVerbose(string messageTemplate, params object[] propertyValues)
        => _serilogLogger.Verbose(messageTemplate, propertyValues);

    public void LogDebug(string messageTemplate, params object[] propertyValues)
        => _serilogLogger.Debug(messageTemplate, propertyValues);

    public void LogInformation(string messageTemplate, params object[] propertyValues)
        => _serilogLogger.Information(messageTemplate, propertyValues);

    public void LogWarning(string messageTemplate, params object[] propertyValues)
        => _serilogLogger.Warning(messageTemplate, propertyValues);

    public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
        => _serilogLogger.Error(exception, messageTemplate, propertyValues);

    public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
        => _serilogLogger.Fatal(exception, messageTemplate, propertyValues);
}