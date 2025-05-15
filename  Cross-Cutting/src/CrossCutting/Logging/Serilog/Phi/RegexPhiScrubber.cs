using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace TheSSS.DicomViewer.Common.Logging.Serilog.Phi;

public class RegexPhiScrubber : Logging.Phi.IPhiScrubber
{
    private readonly IReadOnlyList<Regex> _phiRegexes;
    private readonly string _replacementValue;

    public RegexPhiScrubber(IOptions<PhiScrubberOptions> options)
    {
        var patterns = options.Value.Patterns;
        _replacementValue = options.Value.Replacement;
        
        _phiRegexes = patterns?
            .Select(p => new Regex(p, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            .ToList() ?? new List<Regex>();
    }

    public string Scrub(string messageContent)
    {
        if (string.IsNullOrEmpty(messageContent))
            return messageContent;

        var scrubbedContent = messageContent;
        foreach (var regex in _phiRegexes)
        {
            scrubbedContent = regex.Replace(scrubbedContent, _replacementValue);
        }
        return scrubbedContent;
    }
}