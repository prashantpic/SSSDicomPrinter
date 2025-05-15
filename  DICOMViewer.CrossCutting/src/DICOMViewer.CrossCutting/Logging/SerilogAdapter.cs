namespace TheSSS.DICOMViewer.Common.Logging
{
    public class SerilogAdapter : TheSSS.DICOMViewer.Common.Abstractions.Logging.ILoggerAdapter
    {
        private readonly Serilog.ILogger _logger;

        public SerilogAdapter(Serilog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogVerbose(string messageTemplate, params object[] propertyValues) => _logger.Verbose(messageTemplate, propertyValues);
        public void LogDebug(string messageTemplate, params object[] propertyValues) => _logger.Debug(messageTemplate, propertyValues);
        public void LogInformation(string messageTemplate, params object[] propertyValues) => _logger.Information(messageTemplate, propertyValues);
        public void LogWarning(string messageTemplate, params object[] propertyValues) => _logger.Warning(messageTemplate, propertyValues);
        public void LogError(Exception? exception, string messageTemplate, params object[] propertyValues) => _logger.Error(exception, messageTemplate, propertyValues);
        public void LogFatal(Exception? exception, string messageTemplate, params object[] propertyValues) => _logger.Fatal(exception, messageTemplate, propertyValues);
    }
}