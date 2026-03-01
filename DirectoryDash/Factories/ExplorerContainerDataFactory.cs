using DirectoryDash.Models;
using DirectoryDash.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Factories
{
    internal class ExplorerContainerDataFactory
    {
        public static ExplorerContainerData CreateChildData(ExplorerContainerData parent, string nodePath)
        {
            return new ExplorerContainerData()
            {
                ElementName = Path.GetFileName(nodePath),
                ElementPath = nodePath,
                XCoord = parent.XCoord - parent.Width - 20,
                YCoord = parent.YCoord,
            };
        }
    }
}
