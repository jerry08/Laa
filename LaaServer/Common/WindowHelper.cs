using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LaaServer
{
    public static class WindowHelper
    {
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        [DllImport("User32")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);

        public static void CenterWindowOnScreen(Window windowToCenter)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = windowToCenter.Width;
            double windowHeight = windowToCenter.Height;
            windowToCenter.Left = (screenWidth / 2) - (windowWidth / 2);
            windowToCenter.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        public static void BringProcessToFront(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }

        const int SW_RESTORE = 9;

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
    }
}