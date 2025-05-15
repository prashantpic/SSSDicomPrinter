namespace TheSSS.DICOMViewer.Infrastructure.Persistence.Entities
{
    public class UserSettingDbo
    {
        public int Id { get; set; }
        public string UserIdentifier { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}