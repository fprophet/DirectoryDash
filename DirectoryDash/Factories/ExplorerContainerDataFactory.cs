using DirectoryDash.Enums;
using DirectoryDash.Helpers;
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

        public static ExplorerContainerData CreateChildData(ExplorerContainerData parent, string nodePath, bool isPositionedAtStart)
        {
            double x = 0, y = 0;

            if (isPositionedAtStart)
            {
                x = Vars.StartXCoord;
                y = parent.YCoord;
            }
            else
            {
                x = parent.XCoord - parent.Width;
                y = Vars.StartYCoord;
            }

            return new ExplorerContainerData()
            {
                ElementName = Path.GetFileName(nodePath),
                ElementPath = nodePath,
                XCoord = x,
                YCoord = y,
            };
        }
    }
}
