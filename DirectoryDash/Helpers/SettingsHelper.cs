using DirectoryDash.Models;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.Json;      
using System.Windows.Input;


namespace DirectoryDash.Helpers
{
    internal class SettingsHelper
    {
        public static readonly string Directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Vars.AppName);
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
            try
            {

                var settings = File.ReadAllText(SettingsFile);
                var json = JsonSerializer.Deserialize<Settings>(settings);

                Settings = json ?? new Settings();

                CheckSavedPaths();
            }
            catch (Exception ex)
            {
                CreateDefaultSettings();
            }
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
            Settings = new Models.Settings()
            {
                SavedPaths = new List<string>() { Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
                OnStartup = true,
                DirectoriesOnly = false
            };

            SaveSettings();
        }

        public static void SaveSettings()
        {
            SetStartup(Settings.OnStartup);

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

        public static void SetStartup(bool enable)
        {
            string exePath = Environment.ProcessPath!;

            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (enable)
            {
                key.SetValue(Vars.AppName, $"\"{exePath}\" /startup");
            }
            else
            {
                key.DeleteValue(Vars.AppName, false);
            }
        }
    }
}
