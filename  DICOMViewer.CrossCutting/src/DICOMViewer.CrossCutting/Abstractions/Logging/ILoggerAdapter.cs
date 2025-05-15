namespace TheSSS.DICOMViewer.Common.Abstractions.Logging
{
    public interface ILoggerAdapter
    {
        void LogVerbose(string messageTemplate, params object[] propertyValues);
        void LogDebug(string messageTemplate, params object[] propertyValues);
        void LogInformation(string messageTemplate, params object[] propertyValues);
        void LogWarning(string messageTemplate, params object[] propertyValues);
        void LogError(Exception? exception, string messageTemplate, params object[] propertyValues);
        void LogFatal(Exception? exception, string messageTemplate, params object[] propertyValues);
    }
}