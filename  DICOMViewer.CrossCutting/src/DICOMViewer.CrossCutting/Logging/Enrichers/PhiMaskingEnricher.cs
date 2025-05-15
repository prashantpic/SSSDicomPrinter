namespace TheSSS.DICOMViewer.Common.Logging.Enrichers;

public class PhiMaskingEnricher : ILogEventEnricher
{
    private static readonly HashSet<string> SensitivePropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "User", "PatientId", "AccessionNumber", "PatientName", "DOB", 
        "Address", "Phone", "Email", "PHI", "Password", "Key", "Secret"
    };

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var properties = logEvent.Properties.ToDictionary(p => p.Key, p => p.Value);

        foreach (var property in properties)
        {
            if (SensitivePropertyNames.Contains(property.Key))
            {
                var maskedProperty = propertyFactory.CreateProperty(property.Key, "[PHI_REDACTED]");
                logEvent.AddOrUpdateProperty(maskedProperty);
            }
        }
    }
}