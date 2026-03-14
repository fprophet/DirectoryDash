using DirectoryDash.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DirectoryDash.Helpers
{
    internal class Vars
    {
        public static double ScreenWidth => SystemParameters.PrimaryScreenWidth;

        public static double ScreenHeight => SystemParameters.PrimaryScreenHeight;

        public static double ContainerHeight { get; set; } = 400;

        public static double ContainerWidth { get; set; } = 300;

        public static double StartYCoord { get; set; } = 0;

        public static double StartXCoord { get; set; } = 0;

        public static bool DirectionChanged { get; set; } = false;

        public static Direction Direction { get; set; } = Direction.RTL;

        public static double MouseYPosition { get; set; } = 0;

        public static double MouseXPosition { get; set; } = 0;
        
        public static double ContainerHorMargin { get; set; } = 20;

        public const string AppName = "DirectoryDash";

        internal static void Reset()
        {
            Vars.DirectionChanged = false;
            Direction = Direction.RTL;
        }

        internal static void GetStartCoordinates(double mouseXPosition, double mouseYPosition)
        {
            Vars.MouseYPosition = mouseYPosition;
            Vars.MouseXPosition = mouseXPosition;

            //StartYCoord = MouseYPosition - ContainerHeight - 100;
            //StartXCoord = MouseXPosition - 100;
        }
    }
}
