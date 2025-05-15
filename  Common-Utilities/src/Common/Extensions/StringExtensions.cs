using System.Linq;
using System.Text;

namespace TheSSS.DicomViewer.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool HasNonAsciiChars(this string value)
        {
            return value?.Any(c => c > 127) ?? false;
        }

        public static string NormalizeDiacritics(this string value)
        {
            return new string(value?
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
    }
}