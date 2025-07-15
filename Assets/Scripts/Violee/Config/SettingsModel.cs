namespace Violee
{
    public class SettingsModel : Singleton<SettingsModel>
    {
        public SettingsConfig SettingsConfigIns;
		public static SettingsConfig SettingsConfig => Instance.SettingsConfigIns;
    }
}