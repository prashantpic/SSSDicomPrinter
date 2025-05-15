using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Anonymization
{
    public class AnonymizationProfile
    {
        public AnonymizationProfileId Id { get; init; }
        public string ProfileName { get; private set; }
        public AnonymizationRules MetadataRules { get; private set; }
        public PixelAnonymizationTemplateId? PixelAnonymizationTemplateId { get; private set; }

        public AnonymizationProfile(AnonymizationProfileId id, string profileName, 
            AnonymizationRules metadataRules, PixelAnonymizationTemplateId? pixelTemplateId = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ProfileName = profileName ?? throw new ArgumentNullException(nameof(profileName));
            MetadataRules = metadataRules ?? throw new ArgumentNullException(nameof(metadataRules));
            PixelAnonymizationTemplateId = pixelTemplateId;
        }

        public void UpdateProfile(string newName, AnonymizationRules newRules)
        {
            ProfileName = newName ?? throw new ArgumentNullException(nameof(newName));
            MetadataRules = newRules ?? throw new ArgumentNullException(nameof(newRules));
        }
    }
}