using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Models
{
    internal class Settings
    {
        public string SourcePath { get; set; }

        public bool OnStartup { get; set; } = true;

        public bool FoldersOnly { get; set; } = false;
    }
}
