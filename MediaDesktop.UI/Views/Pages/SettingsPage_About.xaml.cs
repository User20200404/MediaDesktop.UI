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
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage_About : Page
    {
        PackageVersion packVer = Package.Current.Id.Version;
        string AssemblyVer => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string PackageVersion => $"{packVer.Major}.{packVer.Minor}.{packVer.Build}.{packVer.Revision}";
        public SettingsPage_About()
        {
            this.InitializeComponent();
            if(updateLogNavView.MenuItems.Any())
            {
                updateLogContentFrame.Content = updateLogNavView.MenuItems.Last().As<UpdateLogViewModel>().SummaryText;
            }
        }

        private void UpdateLogNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            UpdateLogViewModel viewModel = args.InvokedItem as UpdateLogViewModel;
            updateLogContentFrame.Content = viewModel.SummaryText;
        }
    }
}
