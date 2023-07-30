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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using MediaDesktop.UI.Views.Windows;
using MediaDesktop.UI.ViewModels;
using LibVLCSharp.Shared;
using MediaDesktop.UI.Interfaces;
using System.Buffers;
using Windows.ApplicationModel.DataTransfer;
using MediaDesktop.UI.Helpers;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyItemDialogPage : Page
    {
        /// <summary>
        /// The true viewModel, compared to DataContext that is tempModel. (But DataContext is also true model during adding operation.)
        /// </summary>
        private MediaDesktopItemViewModel trueViewModel;
        private ContentDialog parentDialog;
        private MediaDesktopItemViewModel BoundViewModel { get => DataContext as  MediaDesktopItemViewModel; }
        public ModifyItemDialogPage(MediaDesktopItemViewModel trueViewModel)
        {
            this.InitializeComponent();
            this.Loaded += ModifyItemDialogPage_Loaded;
            this.trueViewModel = trueViewModel;
        }

        private void ModifyItemDialogPage_Loaded(object sender, RoutedEventArgs e)
        {
            parentDialog = this.Parent as ContentDialog;
            UpdateUIStatus();
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            senderButton.IsEnabled = false;
            var targetTextBox = senderButton.Tag as TextBox;

            FileOpenPicker picker = new FileOpenPicker();

            if (targetTextBox == mediaPathTextBox)
            {
                picker.FileTypeFilter.Add(".avi");
                picker.FileTypeFilter.Add(".mp4");
                picker.FileTypeFilter.Add(".mkv");
                picker.FileTypeFilter.Add(".wmv");
                picker.FileTypeFilter.Add(".flv");
            }
            if(targetTextBox == imagePathTextBox)
            {
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".gif");
                picker.FileTypeFilter.Add(".bmp");
            }

            WinUI3Helper.PickerInitializeWindow(ClientWindow.Instance, picker);

            StorageFile storageFile = await picker.PickSingleFileAsync();
            senderButton.IsEnabled = true;
            if (storageFile is null)
                return;

            targetTextBox.Text = storageFile.Path;
        }

        private void UpdateUIStatus()
        {
            if(!File.Exists(mediaPathTextBox.Text))
            {
                notifyTextBlock.Text = "媒体文件不存在。";
                notifyTextBlock.Visibility = Visibility.Visible;
                parentDialog.IsPrimaryButtonEnabled = false;
            }
            else
            {
                notifyTextBlock.Visibility = Visibility.Collapsed;
                parentDialog.IsPrimaryButtonEnabled = true;
            }
        }

        private void MediaPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateUIStatus();
        }
    }
}
