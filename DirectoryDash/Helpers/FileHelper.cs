using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Helpers
{
    class FileHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHELLEXECUTEINFO
        {
            public uint cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        [DllImport("Shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        public const int SEE_MASK_INVOKEIDLIST = 0x0000000C;

        public static void OpenFileProperties(string filePath)
        {
            SHELLEXECUTEINFO sei = new SHELLEXECUTEINFO();
            sei.cbSize = (uint)Marshal.SizeOf(sei);
            sei.lpVerb = "properties";
            sei.lpFile = filePath;
            sei.nShow = 0; // SW_HIDE
            sei.fMask = SEE_MASK_INVOKEIDLIST;
            if (!ShellExecuteEx(ref sei))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}
