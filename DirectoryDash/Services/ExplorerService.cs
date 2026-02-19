using DirectoryDash.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Services
{
    internal class ExplorerService
    {
        public List<ExplorerNode> GetNodes(string path)
        {
            try
            {
                var entires = Directory.GetFileSystemEntries(path);
                var nodes = entires.Select(entry => new ExplorerNode
                {
                    Name = Path.GetFileName(entry),
                    FullPath = entry,
                    IsDirectory = Directory.Exists(entry)
                }).ToList();

                return nodes;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                Trace.WriteLine($"Error accessing path: {ex.Message}");
                return new List<ExplorerNode>();
            }
        }
    }
}
