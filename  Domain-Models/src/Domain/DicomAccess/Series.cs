using System.Collections.Generic;
using System.Collections.ObjectModel;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Series
    {
        private readonly List<Instance> _instances = new();
        
        public SeriesInstanceUID Id { get; }
        public StudyInstanceUID StudyId { get; }
        public IReadOnlyCollection<Instance> Instances => new ReadOnlyCollection<Instance>(_instances);

        public Series(SeriesInstanceUID id, StudyInstanceUID studyId)
        {
            Id = id;
            StudyId = studyId;
        }

        public void AddInstance(Instance instance)
        {
            if (instance.SeriesId != Id)
                throw new InvalidOperationException("Instance belongs to different series");
            _instances.Add(instance);
        }
    }
}