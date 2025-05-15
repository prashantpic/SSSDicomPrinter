namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

public interface IPacsStatusAdapter
{
    Task<IEnumerable<PacsConnectionInfoDto>> GetAllPacsStatusesAsync(CancellationToken cancellationToken);
}