using MediaDesktop.UI.Services;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PickFrameDialogPage : Page
    {
        private MediaDesktopItemViewModel viewModel;
        private ContentDialog parentDialog;
        public PickFrameDialogPage()
        {
            this.InitializeComponent();
        }

        private void ThisPage_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = DataContext as MediaDesktopItemViewModel;
            parentDialog = this.Parent as ContentDialog;
            parentDialog.Closed += ParentDialog_Closed;
            parentDialog.PrimaryButtonClick += ParentDialog_PrimaryButtonClick;
            picker.FilePath = viewModel.MediaPath;

            UpdateUIStatus();
        }

        private void ParentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            picker.Dispose();
            picker = null;
            parentDialog = null;
        }

        private void UpdateUIStatus()
        {
            string expectedPath = picker.GetExpectedExtractFilePath();
            bool any = GlobalResources.ImageCaches.Where(i => i.ImagePath == expectedPath).Any();
            if (any) warningTextBlock.Visibility = Visibility.Visible;
        }
        private void ParentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            viewModel.ImagePath = null;   //Force update.
            viewModel.ImagePath = picker.ExtractedImagePath;
        }
    }
}
