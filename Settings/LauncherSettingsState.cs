using System;

namespace MultiGDLauncher
{
    internal class LauncherSettingsState
    {
        public static string StoragePath
        {
            get
            {
                return LauncherSettingsUtils.GetSettingOrFallback<string>("storage_path", "C:\\MultiGD");
            }
            set
            {
                LauncherSettingsUtils.SaveSetting("storage_path", value);
            }
        }
        public static string SteamGDPath
        {
            get
            {
                return LauncherSettingsUtils.GetSettingOrFallback<string>("steam_gd_path", Steam.Utils.GetDefaultPath());
            }
            set
            {
                LauncherSettingsUtils.SaveSetting("steam_gd_path", value);
            }
        }
    }
}
