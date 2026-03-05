using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Models
{
    internal class Settings
    {
        public List<string> SavedPaths { get; set; } = new List<string>();

        public bool OnStartup { get; set; } = true;

        public bool DirectoriesOnly { get; set; } = false;

        public bool NavigateOnHover { get; set; } = false;
    }
}
