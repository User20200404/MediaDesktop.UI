using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using MediaDesktop.UI.ViewModels;
using IniParser;
using IniParser.Model;
using IniParser.Parser;
using Microsoft.UI.Dispatching;
using MediaDesktop;
using MediaDesktop.UI.Models;
using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MediaDesktop.UI.Helpers;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using MediaDesktop.UI.Views.Windows;
using System.IO;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Media.Editing;

namespace MediaDesktop.UI.Services
{
    public class GlobalResources
    {
        #region Events
        public static event EventHandler InitializeLibVLCCompeleted;
        public static event EventHandler InitializeCompleted;
        #endregion

        #region Global Resources
        public static List<ImageCache> ImageCaches { get; private set; }
        public static MediaDesktopBase MediaDesktopBase { get; private set; }
        public static MediaDesktopPlayer MediaDesktopPlayer { get; private set; }
        public static ViewModelCollection ViewModelCollection { get; private set; }
        public static FileIniDataParser FileIniDataParser { get; private set; }
        public static Views.ViewHelpers.MessageFlyoutHelper MessageFlyoutHelper { get; private set; }
        public static GlobalRoutedEventHelper<PointerRoutedEventArgs, PointerEventHandler> GlobalPointerReleasedEventHelper { get; private set; }
        public static LibVLC LibVLC { get; private set; }

        #endregion

        #region Status
        public static bool IsInitialized
        {
            get { return LibVLC is not null; }
        }

        #endregion

        #region Methods
        public static void RoutedEventInterupter(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Push <paramref name="str"/> to a queue that shows the message to user.
        /// </summary>
        /// <param name="str">The text message to show.</param>
        public static void PushForegroundMessage(string str)
        {
            PushForegroundMessage(str, TimeSpan.FromMilliseconds(1000));
        }
        public static void PushForegroundMessage(string str, TimeSpan holdDuration)
        {
                MessageFlyoutHelper.Enqueue(new Views.ViewHelpers.MessageParameter() { MessageContent = str, HoldDuration = holdDuration });
        }


        /// <summary>
        /// Shows or hides a window using the handle to it.
        /// </summary>
        /// <param name="handle"></param>
        public static void ShowHideWindow(IntPtr handle)
        {
            int nCmdShow;
            if (WindowManager.WinApis.UnEncapsulated.WinApi.IsWindowVisible(handle))
            {
                nCmdShow = 0;
            }
            else
            {
                nCmdShow = 5;
            }

            WindowManager.WinApis.UnEncapsulated.WinApi.ShowWindow(handle, nCmdShow);
        }
        public static void ShowHideMainWindow(bool showNotification = true)
        {
            Services.GlobalResources.ShowHideWindow(Views.Windows.ClientWindow.Instance.Handle);
            if (!ClientWindow.Instance.Visible && showNotification)
            {
                AppNotificationBuilder builder = new AppNotificationBuilder();
                builder.AddArgument("action","ToastClicked");
                builder.AddText("窗口已经隐藏，可以通过单击托盘图标来恢复。");
                builder.AddButton(
                    new AppNotificationButton("显示窗口").
                    AddArgument("action","ShowWindow"));
                builder.AddButton(
                    new AppNotificationButton("明白了").
                    AddArgument("action","OK"));

                var currentViewModel = ViewModelCollection.CurrentDesktopItemViewModel;

                if(currentViewModel is not null)
                {
                    string imagePath = ViewModelCollection.CurrentDesktopItemViewModel.ImagePath;
                    if(File.Exists(imagePath))
                    {
                        builder.SetInlineImage(new Uri(imagePath));
                    }
                    builder.AddText($"当前正在播放的项目：{currentViewModel.Title}");
                }

                AppNotification notification = builder.BuildNotification();
                notification.ExpiresOnReboot = true;
                notification.Expiration = DateTimeOffset.Now.AddMilliseconds(15000);
                AppNotificationManager.Default.Show(notification);
            }
        }

        public static void ExitApplication()
        {
            WindowManager.WinApis.UnEncapsulated.WinApi.PostMessageA(Views.Windows.ClientWindow.Instance.Handle, 16, IntPtr.Zero, IntPtr.Zero);
        }

        public static async void InitializeAsync()
        {
            InitializeLibVLCCompeleted += GlobalResources_InitializeLibVLCCompeleted;
            InitResources();
            InitIniParser();
            InitMediaDesktopHelper();
            InitViewModel();
            InitModel();
            InitHelper();
            await InitLibVLC();
            InitializeCompleted?.Invoke(typeof(GlobalResources), EventArgs.Empty);
        }

        private static void GlobalResources_InitializeLibVLCCompeleted(object sender, EventArgs e)
        {
            MediaDesktopPlayer.LibVLC = LibVLC;
        }
        #endregion
        #region Delegate Commands
        public static DelegateCommand ShowHideMainWindowCommand = new DelegateCommand(obj => ShowHideMainWindow());
        public static DelegateCommand ExitApplicationCommand = new DelegateCommand(obj => ExitApplication());
        #endregion

        #region Inner Methods
        private static void InitHelper()
        {
            GlobalPointerReleasedEventHelper = new(ClientWindow.Instance.appRootPageGrid, UIElement.PointerReleasedEvent);
            MessageFlyoutHelper = new Views.ViewHelpers.MessageFlyoutHelper(ClientWindow.Instance.Compositor, ClientWindow.Instance.rootNotifyPopup);
        }

        private static void InitIniParser()
        {
            FileIniDataParser = new FileIniDataParser();
        }

        private static void InitViewModel()
        {
            ViewModelCollection = new ViewModelCollection();
        }

        private static void InitModel()
        {
            ImageCaches = new List<ImageCache>();
        }

        private static void InitResources()
        {
            ImageCache.DefaultSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Image/DefaultCover.png"));
        }

        private static void InitMediaDesktopHelper()
        {
            MediaDesktopBase = new MediaDesktopBase();
            MediaDesktopPlayer = new MediaDesktopPlayer();
            MediaDesktopPlayer.AttachToMediaDesktopBase(MediaDesktopBase);
            MediaDesktopBase.MediaAttachedPosition = MediaAttachedPosition.AttachToWorkerW;
            MediaDesktopBase.AttachToDesktop();
        }

        private static async Task InitLibVLC()
        {
            await Task.Run(() =>
            {
                //Core.Initialize(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\libvlc\\win-x64");
                LibVLC = new LibVLC();
            });
            InitializeLibVLCCompeleted?.Invoke(LibVLC, EventArgs.Empty);
        }
        #endregion
    }
}
