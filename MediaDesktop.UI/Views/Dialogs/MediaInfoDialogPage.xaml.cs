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

namespace MediaDesktop.UI.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaInfoDialogPage : Page
    {
        bool isShowingOpenConfirmStackPanel = false;
        public MediaInfoDialogPage()
        {
            this.InitializeComponent();
        }

        private void RTB_FilePath_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            textBlock_FilePath.Height = (sender as RichTextBlock)?.ActualHeight ?? 19;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isShowingOpenConfirmStackPanel)
            {
                isShowingOpenConfirmStackPanel = true;
                (Resources["ConfirmGroupStackPanelShowAnimation"] as Storyboard).Begin();
            }
        }
        private void ConfirmButtons_Click(object sender, RoutedEventArgs e)
        {
            (Resources["ConfirmGroupStackPanelHideAnimation"] as Storyboard).Begin();
            var button = sender as Button;
            if (button.Name == "acceptButton")
            {
                var model = button.DataContext as MediaItemViewModel;
                model?.BrowseMediaFileInExplorer();
            }
            isShowingOpenConfirmStackPanel = false;
        }
    }
}
