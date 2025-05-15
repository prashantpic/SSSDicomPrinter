using Prism.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Presentation.Coordinator.Models;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class ViewSynchronizationService : IViewSynchronizationService
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _isSynchronizationActive;

        public ViewSynchronizationService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void RequestSynchronization(SyncParameters parameters)
        {
            if (_isSynchronizationActive)
            {
                _eventAggregator.GetEvent<SynchronizeViewEvent>().Publish(parameters);
            }
        }

        public void SetSynchronizationActive(bool isActive)
        {
            _isSynchronizationActive = isActive;
            _eventAggregator.GetEvent<ToggleSynchronizationStateEvent>().Publish(isActive);
        }

        public bool IsSynchronizationActive() => _isSynchronizationActive;
    }
}