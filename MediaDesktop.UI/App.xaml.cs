using Microsoft.UI.Xaml;
using System;
using MediaDesktop.UI.Views.Windows;
using MediaDesktop.UI.Services;
using System.Diagnostics;
using H.NotifyIcon;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppLifecycle;
using System.Runtime.InteropServices;
using WindowManager.WinApis.UnEncapsulated;
using WindowManager;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        const int WM_SHOWWINDOW = 0x18;
        const int WM_WINDOWPOSCHANGING = 0x46;
        const int WM_GETMINMAXINFO = 0x24;
        ClientWindow m_window;
        SubclassProc subclassProc;
        WndProcCallBack wndHookProc;
        IntPtr hhook;
        GCHandle cBTHookProcHandle;
        GCHandle subClassProcHandle;


        /// <summary>
        /// If false, blocks show window message of main window and its child windows. Should set to true after app initialization.
        /// </summary>
        public static bool AllowShowWindow = false;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            AppNotificationManager.Default.NotificationInvoked += App_NotificationInvoked;
            AppNotificationManager.Default.Register();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            this.UnhandledException += App_UnhandledException;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            AppNotificationManager.Default.Unregister();// Unregister notification COM services.
        }

        private void App_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            //Argument is in the format of "action=...", so we slice the string from index of 7 to get the actual command.
            switch(args.Argument[7..])
            {
                case "ShowWindow": ClientWindow.Instance.Show(); break;
            }
        }



        private unsafe IntPtr CBTWndProcCallBack(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode == 3)
            {
                //Allocates handle for delegate object to prevent being GC.
                subclassProc = SubWndProc;
                subClassProcHandle = GCHandle.Alloc(subclassProc);
                WinApi.SetWindowSubclass(wParam, subclassProc, IntPtr.Zero, IntPtr.Zero);
                SystemAPIs.UnhookWindowsHookEx(hhook);
                hhook = IntPtr.Zero;
                
            }
            return SystemAPIs.CallNextHookEx(IntPtr.Zero,nCode,wParam,lParam);
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            GlobalResources.PushForegroundMessage($"发生内部异常({e.GetType().Name})：\n{e.Message}", TimeSpan.FromMilliseconds(4000));
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            int threadId = SystemAPIs.GetCurrentThreadId();
            wndHookProc = CBTWndProcCallBack;
            cBTHookProcHandle = GCHandle.Alloc(wndHookProc);
            hhook = SystemAPIs.SetWindowsHookExA(5, CBTWndProcCallBack, IntPtr.Zero, threadId);
            Debug.WriteLine($"LastErrorCode = {Marshal.GetLastWin32Error()}");


            var currentInstance = AppInstance.GetCurrent();
            if (currentInstance.IsCurrent)
            {
                AppActivationArguments activationArgs = currentInstance.GetActivatedEventArgs();
                if (activationArgs != null)
                {
                    ExtendedActivationKind kind = activationArgs.Kind;
                    if (kind == ExtendedActivationKind.AppNotification)
                    {
                        var notificationActivatedEventArgs = activationArgs.Data as AppNotificationActivatedEventArgs;
                        string notifyArgString = notificationActivatedEventArgs.Argument;
                        switch (notifyArgString[7..])
                        {
                            default: Environment.Exit(0); break; //Exits the app, as it's activated by clicking notification.
                        }
                    }
                }
            }
            m_window = new ClientWindow();


        }

        IntPtr SubWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData)
        {
            if (uMsg == WM_WINDOWPOSCHANGING && !AllowShowWindow)
            {
                WindowManager.WinApis.UnEncapsulated.tagWINDOWPOS windowPos = Marshal.PtrToStructure<tagWINDOWPOS>(lParam);
                windowPos.flags &= ~(uint)0x40;
                windowPos.flags |= 0x80;
                Marshal.StructureToPtr(windowPos,lParam, true);
            }

            if (uMsg == WM_GETMINMAXINFO)
            {
                tagMINMAXINFO minMaxInfo = Marshal.PtrToStructure<tagMINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize = new POINT() { x = 0x2fe, y = 0x1b5 };
                Marshal.StructureToPtr(minMaxInfo, lParam, true);
            }
            return WinApi.DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }
    }
}
