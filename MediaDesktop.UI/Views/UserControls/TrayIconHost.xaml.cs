using MediaDesktop.UI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.UserControls
{
    public sealed partial class TrayIconHost : UserControl
    {
        DelegateCommand ShowHideMainWindowCommand;
        DelegateCommand ExitApplicationCommand;
        public TrayIconHost()
        {
            this.InitializeComponent();
            InitializeDelegateCommands();
            InitTrayIcon();
        }


        void InitTrayIcon()
        {
            FileStream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Assets/StoreLogo.ico", FileMode.Open);
            trayIcon.Icon = new System.Drawing.Icon(stream);
            stream.Close();
        }
        void InitializeDelegateCommands()
        {
            ShowHideMainWindowCommand = new DelegateCommand(obj => Services.GlobalResources.ShowHideWindow(Views.Windows.ClientWindow.Instance.Handle));
            ExitApplicationCommand = new DelegateCommand(obj => { WindowManager.WinApis.UnEncapsulated.WinApi.PostMessageA(Windows.ClientWindow.Instance.Handle, 16, IntPtr.Zero, IntPtr.Zero); });
        }

    }
}
