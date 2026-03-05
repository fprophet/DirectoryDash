using DirectoryDash.Models;
using System.IO;
using System.Text.Json;


namespace DirectoryDash.Helpers
{
    internal class SettingsHelper
    {
        public static readonly string Directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DirectoryDash");
        public static readonly string SettingsFile = Path.Combine(Directory, "settings.json");

        public static Settings Settings { get; private set; } = new Settings();

        public static void CheckSettings()
        {
            // Ensure the settings directory exists
            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            if (!File.Exists(SettingsFile))
            {
                CreateDefaultSettings();
            }

            LoadSettings();
        }

        private static void LoadSettings()
        {
            var settings = File.ReadAllText(SettingsFile);
            var json = JsonSerializer.Deserialize<Settings>(settings);

            Settings = json ?? new Settings();

            CheckSavedPaths();
        }

        private static void CheckSavedPaths()
        {
            var nonExistent = Settings.SavedPaths.Where(path => !System.IO.Directory.Exists(path)).ToList();

            foreach (var path in nonExistent)
            {
                Settings.SavedPaths.Remove(path);
            }

            SaveSettings();
        }

        private static void CreateDefaultSettings()
        {
            var settings = new Models.Settings()
            {
                SavedPaths = new List<string>() { Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
                OnStartup = true,
                DirectoriesOnly = false
            };

            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(settings));
        }

        public static void SaveSettings()
        {
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(Settings));
        }

        //add condition for directory to exist
        internal static bool AddNavigationPath(string elementPath)
        {
            if (string.IsNullOrEmpty(elementPath) || Settings.SavedPaths.Contains(elementPath))
                return false;

            Settings.SavedPaths.Add(elementPath);
            SaveSettings();
            return true;
        }

        internal static void RemoveNavigationPath(string path)
        {
            Settings.SavedPaths.Remove(path);
            SaveSettings();
        }
    }
}
