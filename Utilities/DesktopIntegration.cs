using System;
using System.Runtime.InteropServices;

namespace UnitDesk.Utilities
{
    /// <summary>
    /// Provides methods for integrating the widget with the Windows desktop environment
    /// </summary>
    public static class DesktopIntegration
    {
        #region Win32 API Imports

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam,
                                              SendMessageTimeoutFlags fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [Flags]
        private enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }

        #endregion

        /// <summary>
        /// Sets a window as a desktop widget by parenting it to the desktop
        /// </summary>
        /// <param name="windowHandle">Window handle to set as desktop widget</param>
        public static void SetWindowToDesktop(IntPtr windowHandle)
        {
            try
            {
                // Get Program Manager
                IntPtr progman = FindWindow("Progman", null);
                IntPtr result = IntPtr.Zero;

                // Send 0x052C to Progman
                SendMessageTimeout(progman,
                                0x052C,
                                new IntPtr(0),
                                IntPtr.Zero,
                                SendMessageTimeoutFlags.SMTO_NORMAL,
                                1000,
                                out result);

                IntPtr workerw = IntPtr.Zero;

                // Find the WorkerW window
                EnumWindows((tophandle, topparamhandle) =>
                {
                    IntPtr p = FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                    if (p != IntPtr.Zero)
                    {
                        workerw = FindWindowEx(IntPtr.Zero,
                                            tophandle,
                                            "WorkerW",
                                            IntPtr.Zero);
                    }

                    return true;
                }, IntPtr.Zero);

                // Set the parent of our window to WorkerW
                SetParent(windowHandle, workerw);
            }
            catch (Exception ex)
            {
                // Log error and handle gracefully
                Console.WriteLine($"Error setting window to desktop: {ex.Message}");
            }
        }
    }
}
