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
        public Action Clear { get; internal set; }

        private CancellationTokenSource _clearViewCT = new CancellationTokenSource();

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

        internal void OpenFile(string fullPath)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = fullPath,
                UseShellExecute = true
            });
        }

        internal async Task StartClear()
        {
            try
            {
                _clearViewCT = new CancellationTokenSource();
                await Task.Delay(2000, _clearViewCT.Token);
                if (!_clearViewCT.IsCancellationRequested)
                    Clear?.Invoke();
            }
            catch (TaskCanceledException) { }
        }

        internal async Task CancelClear() => _clearViewCT.Cancel();
    }
}
