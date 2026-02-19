using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Models
{
    internal class ExplorerNode
    {
        public string Name { get; set; }
        
        public string FullPath { get; set; }
        
        public bool IsDirectory { get; set; }

        public List<ExplorerNode> Children { get; set; } = new List<ExplorerNode>();
    }
}
