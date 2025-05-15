using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Anonymization
{
    public class AnonymizationProfile
    {
        public AnonymizationProfileId Id { get; private set; }
        public string ProfileName { get; private set; }
        public object MetadataRules { get; private set; }
        public PixelAnonymizationTemplateId? PixelAnonymizationTemplateId { get; private set; }

        private AnonymizationProfile() { }

        public AnonymizationProfile(AnonymizationProfileId id, string profileName, object metadataRules, PixelAnonymizationTemplateId? pixelTemplateId = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ProfileName = profileName ?? throw new ArgumentNullException(nameof(profileName));
            MetadataRules = metadataRules ?? throw new ArgumentNullException(nameof(metadataRules));
            PixelAnonymizationTemplateId = pixelTemplateId;
        }
    }
}