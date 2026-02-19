using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Models
{
    internal class ExplorerItem
    {
        public string Name { get; set; }
        
        public string FullPath { get; set; }
        
        public bool IsDirectory { get; set; }
    }
}
