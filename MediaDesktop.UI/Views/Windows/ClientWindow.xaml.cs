using Microsoft.UI.Xaml;
using System;
using MediaDesktop.UI.Services;
using WindowManager;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Windowing;
using H.NotifyIcon;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClientWindow : Window
    {
        public static ClientWindow Instance { get; private set; }
        public static UIElement TitleBarMaskGrid { get { return Instance.borderBackgroundLayerMaskGrid; } }

        public IntPtr Handle { get { return WinRT.Interop.WindowNative.GetWindowHandle(this); } }


        //Attention: Not all the initialization operation is coded in this Ctor. When GlobalResources is initialized, some ini config would be 
        // loaded and notify PropertyChanged event, making its bound property changed.
        // FOR EXAMPLE: arylic and mica effect.

        public ClientWindow()
        {
            Instance = this;
            this.InitializeComponent();

            GlobalResources.InitializeAsync();
            mainContentFrame.Navigate(typeof(Pages.ClientHostPage));
            UpdateTitleBarVisual();
            this.Closed += ClientWindow_Closed;
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(border1);
            Title = "桌面视频播放器(Alpha)";

            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(Handle);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            presenter.IsResizable = true;
            presenter.SetBorderAndTitleBar(true, true);
        }

        /// <summary>
        /// Updates border visual to match the current system theme.
        /// </summary>
        private void UpdateTitleBarVisual()
        {
            var themeColor = (App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush).Color;
            themeColor.R -= 20;
            themeColor.G -= 20;
            themeColor.B -= 20;
            borderBackgroundLayerMaskGrid.Background = new SolidColorBrush(themeColor);
        }

        private void ClientWindow_Closed(object sender, WindowEventArgs args)
        {
            args.Handled = true;
            if (GlobalResources.IsInitialized)
            {
                GlobalResources.MediaDesktopPlayer.TryPause();
                GlobalResources.ViewModelCollection.SettingsItemViewModel.Save();
                GlobalResources.ViewModelCollection.MediaPlayingListConfig.Save();
            }
            var thisWindow = SystemWindow.GetByHandle(this.Handle);
            thisWindow.Hide(); //Hide this window

            MediaDesktopHelper.UpdateWallPaper();
            var mediaPlayerWindow = SystemWindow.GetByHandle(GlobalResources.MediaDesktopBase.AttachmentHandlerForm.Handle);

            Task.Run(() => {
                mediaPlayerWindow.LayeredAttributes.Alpha = 255;
                while (mediaPlayerWindow.LayeredAttributes.Alpha > 0)
                {
                    mediaPlayerWindow.LayeredAttributes.Alpha -= 5;
                    Thread.Sleep(20);
                }
                Environment.Exit(0);
            });
        }
        private void LayerGrid_ThemeChanged(FrameworkElement sender, object args)
        {
            UpdateTitleBarVisual();
        }
    }
}
