using System.Collections.Generic;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Series
    {
        public SeriesInstanceUID Id { get; init; }
        public StudyInstanceUID StudyId { get; init; }
        public string Modality { get; private set; }
        public IReadOnlyCollection<Instance> Instances => _instances.AsReadOnly();
        private readonly List<Instance> _instances = new();

        public Series(SeriesInstanceUID id, StudyInstanceUID studyId, string modality)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            StudyId = studyId ?? throw new ArgumentNullException(nameof(studyId));
            Modality = modality ?? throw new ArgumentNullException(nameof(modality));
        }

        public void AddInstance(Instance instance)
        {
            if (instance?.SeriesId == null || !instance.SeriesId.Equals(Id))
                throw new ArgumentException("Invalid instance assignment");
            
            _instances.Add(instance);
        }
    }
}