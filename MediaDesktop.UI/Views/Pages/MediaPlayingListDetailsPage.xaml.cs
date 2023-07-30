using MediaDesktop.UI.Services;
using MediaDesktop.UI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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

namespace MediaDesktop.UI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaPlayingListDetailsPage : Page
    {
        public MediaPlayingListDetailsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = e.Parameter as MediaPlayingListViewModel;
            base.OnNavigatedTo(e);
        }

        private void backButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (Resources["backButtonMouseEnterAnimation"] as Storyboard).Begin();
        }

        private void backButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (Resources["backButtonMouseLeaveAnimation"] as Storyboard).Begin();
        }

        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as MediaDesktopItemViewModel).MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
        }
    }
}
