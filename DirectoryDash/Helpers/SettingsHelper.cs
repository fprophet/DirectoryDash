using DirectoryDash.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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

            Settings = new Settings()
            {
                SavedPaths = json.SavedPaths,
                OnStartup = json.OnStartup,
                FoldersOnly = json.FoldersOnly
            };
        }

        private static void CreateDefaultSettings()
        {
            var settings = new Models.Settings()
            {
                SavedPaths = new List<string>() { Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
                OnStartup = true,
                FoldersOnly = false
            };

            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(settings));
        }

        public static void SaveSettings()
        {
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(Settings));
        }
    }
}
