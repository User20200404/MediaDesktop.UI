using MediaDesktop.UI.ViewModels;
using MediaDesktop.UI.Services;
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
using Microsoft.UI.Xaml.Shapes;
using System.Windows.Forms;
using MediaDesktop.UI.Interfaces;
using Microsoft.UI.Input;
using Windows.System;
using MediaDesktop.UI.Views.Windows;
using Microsoft.UI.Xaml.Media.Animation;
using System.Numerics;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Pages
{ 
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page,IEventSafeSealed
    {
        private bool isControlKeyDown = false;
        private Grid lastMaskGrid;
        private ViewModelCollection ViewModelCollection { get { return GlobalResources.ViewModelCollection; } }
        public LibraryPage()
        {
            this.InitializeComponent();
            if(!GlobalResources.IsInitialized)
            {
                GlobalResources.InitializeCompleted += GlobalResources_InitializeCompleted;
            }
            else
            {
                StopProgressRingIndicator();
            }

            ((IEventSafeSealed)this).EventStartup();
            playingListFrame.Navigate(typeof(MediaPlayingListPage));
        }

        private void StopProgressRingIndicator()
        {
            loadingProgressRing.IsActive = false;
            loadingProgressRingCover.Visibility = Visibility.Collapsed;
        }

        private void GlobalResources_InitializeCompleted(object sender, EventArgs e)
        {
            StopProgressRingIndicator();
        }

        private void LibraryItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            (element.DataContext as MediaDesktopItemViewModel).MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
        }

        private void LibraryItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

        void IEventSafeSealed.EventStartup()
        {
        }


        void IEventSafeSealed.EventLogOff()
        {
            GlobalResources.InitializeCompleted -= GlobalResources_InitializeCompleted;
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Control)
                isControlKeyDown = true;
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Control)
                isControlKeyDown = false;
        }

        private void Page_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if(isControlKeyDown)
            {
                var pointer = e.GetCurrentPoint(this);
                GlobalResources.ViewModelCollection.SettingsItemViewModel.LibraryItemScale += pointer.Properties.MouseWheelDelta *0.001f;
            }
        }

        private void ItemGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            UIElement targetElement = grid.Tag as UIElement;
            if (targetElement == null)
                return;

            var animation = ClientWindow.Instance.Compositor.CreateVector3KeyFrameAnimation();
            animation.Target = "Translation";
            animation.InsertKeyFrame(1, new Vector3(0, 0, 0));
            animation.Duration = TimeSpan.FromMilliseconds(400);
            targetElement.StartAnimation(animation);
        }

        private void ItemGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            UIElement targetElement = grid.Tag as UIElement;
            if (targetElement == null)
                return;

            var compositor = ClientWindow.Instance.Compositor;
            var animation = compositor.CreateVector3KeyFrameAnimation();
            var function = compositor.CreateCubicBezierEasingFunction(new Vector2(0.1f, 0.9f), new Vector2(0.9f, 0.99f));
            animation.Target = "Translation";
            animation.InsertKeyFrame(1f, new Vector3(0, 0, 100), function);
            animation.Duration = TimeSpan.FromMilliseconds(3000);
            targetElement.StartAnimation(animation);
        }

        private void LibraryItem_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var rect = sender as Rectangle;
            var maskGrid = rect.Tag as Grid;
            var pointer = e.GetCurrentPoint(rect);
            //rect.CapturePointer(e.Pointer); This methods will override pivot's drag opreation. In order to handle PointerReleased event, use global event instead.
            if (pointer.Properties.IsRightButtonPressed)
                ShowItemMenu(rect, pointer.Position);
            if(pointer.Properties.IsLeftButtonPressed)
                maskGrid.Visibility = Visibility.Visible;

            lastMaskGrid = maskGrid;
            GlobalResources.GlobalPointerReleasedEventHelper.RoutedEventTriggered += LibraryItem_PointerReleased;
        }

        private void ShowItemMenu(FrameworkElement element, Point position)
        {
            var flyout = Resources["LibraryItemRightTapMenu"] as MenuFlyout;
            flyout.ShowAt(element, position);
        }

        private void LibraryItem_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            lastMaskGrid.Visibility = Visibility.Collapsed;
            GlobalResources.GlobalPointerReleasedEventHelper.RoutedEventTriggered -= LibraryItem_PointerReleased;
        }

        private void LibraryPageRootGrid_DragEnter(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
                dragReceiverBorder.Visibility = Visibility.Visible;
        }

        private void LibraryPageRootGrid_DragLeave(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            dragReceiverBorder.Visibility = Visibility.Collapsed;
        }

        private void LibraryDragMenuItemRelativePanel0_DragEnter(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
                e.AcceptedOperation = DataPackageOperation.Copy;
            (Resources["DragMenuItem0DragEnterAnimation"] as Storyboard).Begin();
        }

        private void LibraryDragMenuItemRelativePanel0_DragLeave(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            (Resources["DragMenuItem0DragLeaveAnimation"] as Storyboard).Begin();
        }

        private void LibraryDragMenuItemRelativePanel1_DragEnter(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
                e.AcceptedOperation = DataPackageOperation.Copy;
            (Resources["DragMenuItem1DragEnterAnimation"] as Storyboard).Begin();
        }

        private void LibraryDragMenuItemRelativePanel1_DragLeave(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            (Resources["DragMenuItem1DragLeaveAnimation"] as Storyboard).Begin();
        }

        private async void LibraryDragMenuItemRelativePanel0_DropAsync(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                foreach (IStorageItem item in items)
                {
                    ViewModelCollection.ViewModelItems.Add(new MediaDesktopItemViewModel() { MediaPath = item.Path, Title = item.Name });
                }
            }
            LibraryDragMenuItemRelativePanel0_DragLeave(sender, e);
            dragReceiverBorder.Visibility = Visibility.Collapsed;
        }

        private void LibraryDragMenuItemRelativePanel1_DropAsync(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {

        }

    }
}
