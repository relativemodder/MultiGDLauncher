using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Steam
{
    public static partial class Utils
    {
        /// <summary>
        /// Find default path for Steam installation
        /// </summary>
        /// <returns>Directory path to the Steam or empty string</returns>
        public static string GetSteamPath()
        {
            string keyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";
            string valueName = "InstallPath";
            string? newPath = Registry.GetValue(keyPath, valueName, null) as string;
            return newPath ?? string.Empty;
        }

        /// <summary>
        /// Find default path for Geometry Dash (look for Steam installation)
        /// </summary>
        /// <returns>Path to the game directory or empty string</returns>
        public static string GetDefaultPath()
        {
            string steamPath = GetSteamPath();
            if (string.IsNullOrEmpty(steamPath))
                return string.Empty;

            // Get "/steamapps/libraryfolders.vdf" file
            string libraryPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(libraryPath))
                return string.Empty;

            // Find all library paths
            var libraryPaths = LibraryPathsRegex().Matches(File.ReadAllText(libraryPath));
            foreach (Match match in libraryPaths.Cast<Match>())
            {
                string path = match.Groups[1].Value;
                if (Directory.Exists(path))
                {
                    string gdPath = Path.Combine(path, "steamapps", "common", "Geometry Dash").Replace("\\\\", "\\");
                    if (Directory.Exists(gdPath))
                        return gdPath;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Check whether the directory contains "Geode.dll" file
        /// </summary>
        /// <param name="path">Path to the game directory</param>
        /// <returns>Whether the directory contains Geode.dll file</returns>
        public static bool CheckGeode(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return File.Exists(Path.Combine(path, "Geode.dll"));
        }

        [GeneratedRegex(@"""path""\t\t""(.*?)""")]
        private static partial Regex LibraryPathsRegex();
    }
}