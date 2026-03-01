using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Helpers
{
    public static class FileIconHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr handle);

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags);

        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;
        public const uint SHGFI_SMALLICON = 0x1;

        public static Icon GetSmallIcon(string path)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            SHGetFileInfo(path, 0, out shinfo,
                (uint)Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_SMALLICON);

            var icon = Icon.FromHandle(shinfo.hIcon);
            var clone = (Icon)icon.Clone();
            DestroyIcon(shinfo.hIcon);

            return clone;
        }
    }
}
