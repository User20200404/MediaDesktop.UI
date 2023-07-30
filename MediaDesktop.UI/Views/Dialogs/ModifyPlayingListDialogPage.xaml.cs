using MediaDesktop.UI.Helpers;
using MediaDesktop.UI.Views.Windows;
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
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyPlayingListDialogPage : Page
    {
        public ModifyPlayingListDialogPage()
        {
            this.InitializeComponent();
        }


        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            var targetTextBox = senderButton.Tag as TextBox;
            senderButton.IsEnabled = false;

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".bmp");

            WinUI3Helper.PickerInitializeWindow(ClientWindow.Instance, picker);
            StorageFile file = await picker.PickSingleFileAsync();
            senderButton.IsEnabled = true;
            if (file is not null)
            {
                targetTextBox.Text = file.Path;
            }
        }
    }
}
