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

            GetSettings();
        }

        private static void GetSettings()
        {
            var settings = File.ReadAllText(SettingsFile);
            var json = JsonSerializer.Deserialize<Models.Settings>(settings);

            Settings = new Settings()
            {
                SourcePath = json.SourcePath,
                OnStartup = json.OnStartup,
                FoldersOnly = json.FoldersOnly
            };
        }

        private static void CreateDefaultSettings()
        {
            var settings = new Models.Settings()
            {
                SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                OnStartup = true,
                FoldersOnly = false
            };

            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(settings));
        }
    }
}
