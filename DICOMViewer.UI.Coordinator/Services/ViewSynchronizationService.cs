using Prism.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Events;
using TheSSS.DICOMViewer.Presentation.Coordinator.Interfaces.Services;
using TheSSS.DICOMViewer.Common.Interfaces;

namespace TheSSS.DICOMViewer.Presentation.Coordinator.Services
{
    public class ViewSynchronizationService : IViewSynchronizationService
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _isActive;

        public ViewSynchronizationService(IEventAggregator eventAggregator, ILoggerAdapter logger)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ToggleSynchronizationStateEvent>().Subscribe(SetSynchronizationActive);
        }

        public void RequestSynchronization(SyncParameters parameters)
        {
            if (_isActive)
                _eventAggregator.GetEvent<SynchronizeViewEvent>().Publish(parameters);
        }

        public void SetSynchronizationActive(bool isActive) => _isActive = isActive;
        public bool IsSynchronizationActive() => _isActive;
    }
}