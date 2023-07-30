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
using MediaDesktop.UI.Interfaces;
using MediaDesktop.UI.ViewModels;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddToPlayingListDialogPage : Page, IEventSafeSealed
    {
        private ContentDialog parentDialog;
        private MediaPlayingListViewModel listViewModel;
        private MediaDesktopItemViewModel itemViewModel;

        private string NotifyText
        {
            get { return statusTextBlock.Text; }
            set { statusTextBlock.Text = value; }
        }

        /// <summary>
        /// This page should only be used as a <see cref="ContentDialog"/>'s content, and its parent is the dialog instance.
        /// </summary>
        public AddToPlayingListDialogPage()
        {
            this.InitializeComponent();
            this.Loaded += AddToPlayingListDialogPage_Loaded;
        }


        private void AddToPlayingListDialogPage_Loaded(object sender, RoutedEventArgs e)
        {
            parentDialog = this.Parent as ContentDialog;
            itemViewModel = Tag as MediaDesktopItemViewModel;
            parentDialog.IsPrimaryButtonEnabled = false;
            (this as IEventSafeSealed).EventStartup();
            UpdateSelectionStatus();
        }

        void IEventSafeSealed.EventStartup()
        {
            parentDialog.PrimaryButtonClick += ParentDialog_PrimaryButtonClick;
            parentDialog.Closed += ParentDialog_Closed;
            parentDialog.Closing += ParentDialog_Closing;
            listView.SelectionChanged += ListView_SelectionChanged;
        }
        void IEventSafeSealed.EventLogOff()
        {
            parentDialog.PrimaryButtonClick -= ParentDialog_PrimaryButtonClick;
            parentDialog.Closed -= ParentDialog_Closed;
            parentDialog.Closing -= ParentDialog_Closing;
            listView.SelectionChanged -= ListView_SelectionChanged;
        }
        private void ParentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (itemViewModel is not null && listViewModel is not null)
            {
                listViewModel.MediaItems.Add(itemViewModel);
            }
            else
            {
                UpdateSelectionStatus();
                args.Cancel = true; //Data does not meet the requirements, this may be an error, should prevent closing.
            }
        }

        private void ParentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {

        }



        private void ParentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            ((IEventSafeSealed)this).EventLogOff();
        }

        private void UpdateSelectionStatus()
        {
            parentDialog.IsPrimaryButtonEnabled = true;
            NotifyText = "";
            if (listView.Items.Count == 0)
            {
                NotifyText = "你还没有创建任何播放列表，点击下方按钮来新建。";
                parentDialog.IsPrimaryButtonEnabled = false;
                return;
            }


            if (listViewModel is not null)
            {
                if (listViewModel.MediaItems.Contains(itemViewModel))
                {
                    parentDialog.IsPrimaryButtonEnabled = false;
                    NotifyText = "指定的播放列表已包含该项目。";
                }
            }
            else {NotifyText = "未选中列表。"; parentDialog.IsPrimaryButtonEnabled = false; }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listViewModel = e.AddedItems.First() as MediaPlayingListViewModel;
            UpdateSelectionStatus();
        }

        private async void AddNewPlayingListButton_Clicked(object sender, RoutedEventArgs e)
        {
            parentDialog.Hide();
            await Services.GlobalResources.ViewModelCollection.AddPlayingListViewModel(Pages.ClientHostPage.Instance.XamlRoot);
            await parentDialog.ShowAsync();
        }
    }
}
