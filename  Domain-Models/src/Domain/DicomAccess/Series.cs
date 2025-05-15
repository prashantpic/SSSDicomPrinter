using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.DicomAccess
{
    public class Series
    {
        private readonly List<Instance> _instances = new List<Instance>();

        public SeriesInstanceUID Id { get; private set; }
        public StudyInstanceUID StudyId { get; private set; }
        public string Modality { get; private set; }
        public IReadOnlyCollection<Instance> Instances => _instances.AsReadOnly();

        private Series() { }

        public Series(SeriesInstanceUID id, StudyInstanceUID studyId, string modality)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            StudyId = studyId ?? throw new ArgumentNullException(nameof(studyId));
            Modality = modality ?? throw new ArgumentNullException(nameof(modality));
        }

        public void AddInstance(Instance instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (instance.SeriesId != Id) throw new InvalidOperationException("Cannot add instance from different series");
            if (!_instances.Any(i => i.Id == instance.Id))
            {
                _instances.Add(instance);
            }
        }
    }
}