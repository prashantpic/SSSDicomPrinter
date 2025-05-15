using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Configuration
{
    public class UserPreference
    {
        public UserSettingId Id { get; init; }
        public string UserIdentifier { get; init; }
        public string SettingKey { get; private set; }
        public string SettingValue { get; private set; }

        public UserPreference(UserSettingId id, string userIdentifier, string key, string value)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            UserIdentifier = userIdentifier ?? throw new ArgumentNullException(nameof(userIdentifier));
            SettingKey = key ?? throw new ArgumentNullException(nameof(key));
            SettingValue = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}