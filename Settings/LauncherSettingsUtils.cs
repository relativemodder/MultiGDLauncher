using Newtonsoft.Json.Linq;
using System.IO;

namespace MultiGDLauncher
{
    internal class LauncherSettingsUtils
    {

        private static JObject settings_;
        private static JObject settings
        {
            get
            {
                if (settings_ == null) return settings_ = GetSettings();
                return settings_;
            }
            set
            {
                settings_ = value;
            }
        }

        public static void SaveJSONSettings(string contents)
        {
            File.WriteAllText("settings.json", contents);
        }

        public static string GetJSONSettings()
        {
            try
            {
                return File.ReadAllText("settings.json");
            }
            catch (FileNotFoundException)
            {
                SaveJSONSettings("{}");
                return "{}";
            }
        }

        public static JObject GetSettings()
        {
            return JObject.Parse(GetJSONSettings());
        }

        public static T GetSettingOrFallback<T>(string key, string defaultValue)
        {
            if (!settings.ContainsKey(key))
            {
                settings[key] = defaultValue;
            }
            return settings[key].Value<T>();
        }

        public static void SaveSetting(string key, object value)
        {
            settings[key] = JToken.FromObject(value);
            SaveJSONSettings(settings.ToString());
        }
    }
}