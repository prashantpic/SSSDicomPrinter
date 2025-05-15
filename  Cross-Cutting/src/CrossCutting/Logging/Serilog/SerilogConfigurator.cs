using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System.Globalization;

namespace TheSSS.DicomViewer.Common.Logging.Serilog;

public static class SerilogConfigurator
{
    public static ILogger CreateLogger(IConfiguration configuration, Logging.Phi.IPhiScrubber phiScrubber)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.With(new PhiScrubbingEnricher(phiScrubber));

        return loggerConfiguration.CreateLogger();
    }

    private class PhiScrubbingEnricher : ILogEventEnricher
    {
        private readonly Logging.Phi.IPhiScrubber _phiScrubber;

        public PhiScrubbingEnricher(Logging.Phi.IPhiScrubber phiScrubber)
        {
            _phiScrubber = phiScrubber;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.MessageTemplate.Text != null)
            {
                var scrubbedMessage = _phiScrubber.Scrub(logEvent.MessageTemplate.Text);
                logEvent.MessageTemplate = new MessageTemplate(scrubbedMessage, logEvent.MessageTemplate.Tokens);
            }
        }
    }
}