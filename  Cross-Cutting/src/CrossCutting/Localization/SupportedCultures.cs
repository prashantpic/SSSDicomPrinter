namespace TheSSS.DicomViewer.Common.Localization;

public static class SupportedCultures
{
    public static readonly System.Globalization.CultureInfo EnglishUS = new("en-US");
    public static readonly System.Globalization.CultureInfo SpanishES = new("es-ES");
    public static readonly System.Globalization.CultureInfo DefaultCulture = EnglishUS;
    public static readonly System.Collections.Generic.IReadOnlyList<System.Globalization.CultureInfo> All = 
        new System.Collections.Generic.List<System.Globalization.CultureInfo> { EnglishUS, SpanishES }.AsReadOnly();
}