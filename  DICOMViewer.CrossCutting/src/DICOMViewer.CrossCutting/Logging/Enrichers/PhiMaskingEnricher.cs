namespace TheSSS.DICOMViewer.Common.Logging.Enrichers
{
    public class PhiMaskingEnricher : ILogEventEnricher
    {
        private static readonly HashSet<string> _phiKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "PatientId", "PatientName", "PatientBirthDate", "PatientSex",
            "AccessionNumber", "StudyInstanceUID", "SeriesInstanceUID", "SOPInstanceUID"
        };

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var properties = logEvent.Properties.ToDictionary(p => p.Key, p => p.Value);

            foreach (var prop in properties)
            {
                if (_phiKeywords.Contains(prop.Key))
                {
                    var maskedProp = propertyFactory.CreateProperty(prop.Key, "[PHI_REDACTED]");
                    logEvent.AddOrUpdateProperty(maskedProp);
                }
            }
        }
    }
}