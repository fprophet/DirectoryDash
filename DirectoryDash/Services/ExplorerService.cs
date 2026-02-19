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
        public List<ExplorerItem> GetNodes(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Trace.WriteLine($"Directory does not exist: {path}");
                }

                var entires = Directory.GetFileSystemEntries(path);
                var nodes = entires.Select(entry => new ExplorerItem
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
                return new List<ExplorerItem>();
            }
        }
    }
}
