using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace MediaDesktop
{
    /// <summary>
    /// Provides the methods for checking availability of components.
    /// </summary>
    internal class Validation
    {

        /// <summary>
        /// Get the handle of "Program Manager" window (belongs to explorer.exe, with Microsoft Windows signature).
        /// If "Program Manager" is not found, this method throws <see cref="ProgramManagerNotFoundException"/>. 
        /// </summary>
        /// <returns>The <see cref="IntPtr"/> (window handle) of the "Program Manager" window.</returns>
        /// <exception cref="ProgramManagerNotFoundException"></exception>
        public static IntPtr GetProgramManagerWindow()
        {
            List<IntPtr> progManHandles = APIsEncapsulated.FindProgramManagers();
            foreach (IntPtr progManHandle in progManHandles)
            {
               Process explorerProcess =  APIsEncapsulated.GetProcessByWindowHandle(progManHandle);
                if (explorerProcess != null)
                {
                    X509Certificate cert = X509Certificate.CreateFromSignedFile(explorerProcess.MainModule.FileName);
                    if (cert.Subject == "CN=Microsoft Windows, O=Microsoft Corporation, L=Redmond, S=Washington, C=US")
                    {
                        return progManHandle;
                    }
                }
            }

            throw new ProgramManagerNotFoundException("The valid \"Program Manager\" window, which belongs to explorer.exe signed by Microsoft Windows , is not found.");
        }

        /// <summary>
        /// Get the handle of "WorkerW" window.
        /// If "WorkerW" is not found, this method throws <see cref="WorkerWNotFoundException"/>. 
        /// </summary>
        /// <returns>The <see cref="IntPtr"/> (window handle) of the "Program Manager" window.</returns>
        /// <exception cref="WorkerWNotFoundException"></exception>
        public static IntPtr GetWorkerWWindow()
        {
            //The targeted "Worker W" window is created only after the message "1324" is sent to "Program Manager" window.
            //This window owns the following properties : 1.Visible 2.Disabled 3.Belongs to explorer.exe 4.Top desktop window. 5.Class name is "WorkerW" , Title is String.Empty
            //With these features we can find it using :

            List<IntPtr> workerWHanles = APIsEncapsulated.FindWorkerWs();
            foreach(IntPtr workerWHandle in workerWHanles)
            {
                if(!SystemAPIs.IsWindowEnabled(workerWHandle) && SystemAPIs.IsWindowVisible(workerWHandle))
                {
                    return workerWHandle;
                }
            }

            throw new WorkerWNotFoundException("The valid \"WorkerW\" window, which matches the following conditions, is not found. \n\r  1.Visible 2.Disabled 3.Belongs to explorer.exe 4.Top desktop window. 5.Class name is \"WorkerW\" , Title is String.Empty");
        }
    }
}
