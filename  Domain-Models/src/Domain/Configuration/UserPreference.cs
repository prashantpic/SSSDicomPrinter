using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Configuration
{
    public class UserPreference
    {
        public UserSettingId Id { get; private set; }
        public string UserIdentifier { get; private set; }
        public string SettingKey { get; private set; }
        public string SettingValue { get; private set; }

        private UserPreference() { }

        public UserPreference(UserSettingId id, string userIdentifier, string settingKey, string settingValue)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            UserIdentifier = userIdentifier ?? throw new ArgumentNullException(nameof(userIdentifier));
            SettingKey = settingKey ?? throw new ArgumentNullException(nameof(settingKey));
            SettingValue = settingValue;
        }

        public void UpdateValue(string newValue)
        {
            SettingValue = newValue;
        }
    }
}