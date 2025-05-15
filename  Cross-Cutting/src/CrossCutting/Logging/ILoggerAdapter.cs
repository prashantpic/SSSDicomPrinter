namespace TheSSS.DicomViewer.Common.Logging;

public interface ILoggerAdapter
{
    void LogVerbose(string messageTemplate, params object[] propertyValues);
    void LogDebug(string messageTemplate, params object[] propertyValues);
    void LogInformation(string messageTemplate, params object[] propertyValues);
    void LogWarning(string messageTemplate, params object[] propertyValues);
    void LogError(System.Exception exception, string messageTemplate, params object[] propertyValues);
    void LogFatal(System.Exception exception, string messageTemplate, params object[] propertyValues);
}