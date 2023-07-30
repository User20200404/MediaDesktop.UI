using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace MediaDesktop
{
    public struct WIN32tagCWPRETSTRUCT
    {
       public int lResult;
       public uint lParam;
       public uint wParam;
       public uint message;
       public IntPtr hwnd;
    }

    public class APIsEncapsulated
    {
        /// <summary>
        /// Find all "Program Manager" windows' handle.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="IntPtr"/> that contains all handles found.</returns>
        public static List<IntPtr> FindProgramManagers()
        {
            return FindTopWindows("Progman", "Program Manager");
        }

        public static List<IntPtr> FindWorkerWs()
        {
            return FindTopWindows("WorkerW", "");
        }

        public static List<IntPtr> FindTopWindows(string className,string windowText)
        {
            IntPtr lastWindowFound = IntPtr.Zero;
            IntPtr parentWindow = IntPtr.Zero;   //we are finding top windows, so the parent window handle should be NULL in C++,which equals to IntPtr.Zero in C#.
            List<IntPtr> windowHandles = new List<IntPtr>();

            while(true)
            {
                lastWindowFound = SystemAPIs.FindWindowExA(parentWindow, lastWindowFound, className, windowText);
                if (lastWindowFound == IntPtr.Zero)
                    break;

                windowHandles.Add(lastWindowFound);
            }

            return windowHandles;
        }
        public static Process GetProcessByWindowHandle(IntPtr windowHandle)
        {
            SystemAPIs.GetWindowThreadProcessId(windowHandle, out int processId);
            return Process.GetProcessById(processId);
        }

        public static int GetThreadIdByWindowHandle(IntPtr windowHandle)
        {
             int tid = SystemAPIs.GetWindowThreadProcessId(windowHandle, out _);
            return tid;
        }

        public static void GetScreenResolution(out int width,out int height)
        {
            width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            //height = Screen.PrimaryScreen.Bounds.Height;
            //width = Screen.PrimaryScreen.Bounds.Width;
        }

        public static Boolean ShowWindow(IntPtr windowHandle)
        {
            return SystemAPIs.ShowWindow(windowHandle, 8);
        }

        public static Boolean HideWindow(IntPtr windowHandle)
        {
            return SystemAPIs.ShowWindow(windowHandle, 0);
        }
    }
}
