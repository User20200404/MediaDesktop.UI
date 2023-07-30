using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using H.NotifyIcon;
using MediaDesktop.UI.Services;
using MediaDesktop.UI.ViewModels;
using Microsoft.UI.Xaml.Media.Animation;
using MediaDesktop.UI.Views.Windows;
using System.Threading.Tasks;
using System.Reflection.Metadata;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClientHostPage : Page
    {
        bool isSlidingTemp = false;
        bool isPlayingTemp = false;
        private MediaDesktopItemViewModel CurrentDesktopItemViewModel 
        {
            get
            {
                if (GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel != null)
                {
                    return GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel;
                }
                else
                {
                    return null;
                }
            }
        }
        private MediaItemViewModel CurrentMediaItemViewModel
        {
            get
            {
                if (CurrentDesktopItemViewModel != null)
                {
                    return GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel.MediaItemViewModel;
                }
                else
                {
                    return null;
                }
            }
        }
        private MediaItemViewModel.RuntimeData CurrentMediaRuntimeDataSet
        {
            get
            {
                if (CurrentMediaItemViewModel != null)
                {
                    return GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel.MediaItemViewModel.RuntimeDataSet;
                }
                else
                {
                    return null;
                }
            }
        }

        public static ClientHostPage Instance { get; set; }
        public ClientHostPage()
        {
            Instance = this;
            this.InitializeComponent();
            this.Loaded += ClientHostPage_Loaded;
            GlobalResources.ViewModelCollection.PropertyChanged += ViewModelCollection_PropertyChanged;
            GlobalResources.MediaDesktopPlayer.ScreenSolutionChanged += System_ScreenSolutionChanged;
            GlobalResources.InitializeLibVLCCompeleted += GlobalResources_InitializeLibVLCCompeleted;

            //ProgressSlider press and release event may not trigger by default, we have to manually add event handler to get its progress value.
            progressSlider.AddHandler(PointerPressedEvent,new PointerEventHandler(ProgressSlider_PointerPressed), true); 
            progressSlider.AddHandler(PointerReleasedEvent,new PointerEventHandler(ProgressSlider_PointerReleased), true);

            (controlGroupBorder.RenderTransform as TranslateTransform).Y = controlGroupBorder.ActualHeight;
        }

        private void ClientHostPage_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(LibraryPage), null, new DrillInNavigationTransitionInfo());

            Task.Run(() => {
                SystemAPIs.SendMessageA(ClientWindow.Instance. Handle, 0x0F, IntPtr.Zero, IntPtr.Zero);
                Task.Delay(200);
                App.AllowShowWindow = true;
                ClientWindow.Instance.Show();
            });
        }


        private void GlobalResources_InitializeLibVLCCompeleted(object sender, EventArgs e)
        {
            AnimateControls();
        }

        private void AnimateControls()
        {
            (controlGroupBorder.RenderTransform as TranslateTransform).Y = controlGroupBorder.ActualHeight;
            (Resources["ControlPanelShowAnimation"] as Storyboard).Begin();
            controlGroupBorder.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Invoked when screen solution changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void System_ScreenSolutionChanged(object sender, EventArgs e)
        {
            //To ensure the playing media to fill the screen, reset its StrechMode to resize properly.
            if (CurrentMediaItemViewModel is not null)
                CurrentMediaItemViewModel.RuntimeDataSet.StrechMode = MediaItemViewModel.MediaStrechMode.UniformToFill;
        }

        private void ViewModelCollection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel):
                    if(CurrentMediaItemViewModel is not null)
                    CurrentMediaItemViewModel.RuntimeDataSet.StrechMode = MediaItemViewModel.MediaStrechMode.UniformToFill;
                    return;
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isSlidingTemp && CurrentMediaRuntimeDataSet != null)
            {
                float value = (float)(sender as Slider).Value;
                if (value > 0.998f)
                    value = 0.998f;
                CurrentMediaRuntimeDataSet.MediaPlayedProgress = value;
            }
        }

        private void ProgressSlider_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isSlidingTemp = true;
            isPlayingTemp = CurrentMediaRuntimeDataSet?.IsMediaPlaying ?? false;
            ProgressSlider_ValueChanged(progressSlider, null);
        }

        private void ProgressSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isSlidingTemp = false;
            if (isPlayingTemp)
            {
                CurrentMediaItemViewModel?.ToggleMediaStatusTo(GlobalResources.MediaDesktopPlayer, MediaItemViewModel.ToggleMediaStatusAction.Play);
            }
            else
            {
                CurrentMediaItemViewModel?.ToggleMediaStatusTo(GlobalResources.MediaDesktopPlayer, MediaItemViewModel.ToggleMediaStatusAction.Pause);
            }
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            DrillInNavigationTransitionInfo info = new DrillInNavigationTransitionInfo();
            switch ((args.InvokedItemContainer as NavigationViewItem).Name)
            {
                case nameof(navigationViewItem_Lib):
                    if (contentFrame.Content is not LibraryPage)
                        contentFrame.Navigate(typeof(LibraryPage),null, info);
                    break;
                case nameof(leftNavigationView.SettingsItem):
                    if (contentFrame.Content is not SettingsPage)
                        contentFrame.Navigate(typeof(SettingsPage), null,info);
                    break;
                case nameof(navigationViewItem_Play):
                    if (contentFrame.Content is not CurrentPlayingListPage)
                        contentFrame.Navigate(typeof(CurrentPlayingListPage), null,info);
                    break;
            }
        }

        /// <summary>
        /// This event is used to manually handle the size of <see cref="navigationViewBackgroundLayerMaskGrid"/>, as binding to actualsize does not make effect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationViewItem_Play_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            navigationViewBackgroundLayerMaskGrid.Width = e.NewSize.Width;
        }

        private void ShowControlPanelButton_Click(object sender, RoutedEventArgs e)
        {
            (Resources["ControlPanelShowAnimation"] as Storyboard).Begin();
            pageBackgroundLayerMaskGrid.SetValue(Grid.RowSpanProperty, 1);
            contentFrame.SetValue(Grid.RowSpanProperty, 1);
            showControlPanelButton.Visibility = Visibility.Collapsed;
        }

        private void HideControlPanelButton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyBoard = Resources["ControlPanelHideAnimation"] as Storyboard;
            controlPanelHideEndKeyFrame.Value = controlGroupBorder.ActualHeight;
            storyBoard.Begin();
            pageBackgroundLayerMaskGrid.SetValue(Grid.RowSpanProperty, 2);
            contentFrame.SetValue(Grid.RowSpanProperty, 2);
            showControlPanelButton.Visibility = Visibility.Visible;
        }
    }
}
