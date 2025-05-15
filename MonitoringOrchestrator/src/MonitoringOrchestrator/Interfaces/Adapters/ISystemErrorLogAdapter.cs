namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

public interface ISystemErrorLogAdapter
{
    Task<SystemErrorInfoSummaryDto> GetCriticalErrorSummaryAsync(TimeSpan lookbackPeriod, CancellationToken cancellationToken);
}