using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MediaDesktop
{
    public class MediaDesktopHelper
    {
        /// <summary>
        /// Terminate explorer.exe. This method will kill the explorer process attached to, instead of all the explorer.exe. 
        /// </summary>
        /// <param name="mediaDesktopBase">A instance of <see cref="MediaDesktopBase"/> for targeting explorer.exe</param>
        public static void KillExplorer(MediaDesktopBase mediaDesktopBase)
        {
            Process target = mediaDesktopBase.ExplorerProcessAttached;
            target.Kill();
        }

        /// <summary>
        /// Terminate explorer.exe.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public static void KillExplorer()
        {
            Process[] processes = Process.GetProcessesByName("explorer");
            foreach (Process explorerProcess in processes)
            {
                X509Certificate cert = X509Certificate.CreateFromSignedFile(explorerProcess.MainModule.FileName);
                if (cert.Subject == "CN=Microsoft Windows, O=Microsoft Corporation, L=Redmond, S=Washington, C=US")
                    explorerProcess.Kill();
            }
        }

        /// <summary>
        /// Start explorer.exe
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public static void StartExplorer()
        {
            Process.Start("explorer.exe");
        }

        /// <summary>
        /// Terminate explorer.exe and restart it.
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public static void RestartExplorer()
        {
            KillExplorer();
            StartExplorer();
        }

        public static void UpdateWallPaper()
        {
            SystemAPIs.SystemParametersInfoA(0x14, 0, null, 0x2);
        }
    }
}
