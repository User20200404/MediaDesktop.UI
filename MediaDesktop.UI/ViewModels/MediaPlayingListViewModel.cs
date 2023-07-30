using MediaDesktop.UI.Models;
using LibVLCSharp.Shared;
using System;
using MediaDesktop.UI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using MediaDesktop.UI.Views.Windows;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using MediaDesktop.UI.Views.Pages;
using Microsoft.UI.Xaml.Media.Animation;

namespace MediaDesktop.UI.ViewModels
{
    public class MediaPlayingListViewModel : INotifyPropertyChanged
    {
        #region Raw Data
        MediaPlayingList mediaPlayingList;
        #endregion
        #region Data Encapsulated
        public string Title
        {
            get { return mediaPlayingList.Title; }
            set
            {
                if (mediaPlayingList.Title != value)
                {
                    mediaPlayingList.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Description
        {
            get { return mediaPlayingList.Description; }
            set
            {
                if (mediaPlayingList.Description != value)
                {
                    mediaPlayingList.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string CoverImagePath
        {
            get { return mediaPlayingList.CoverImagePath; }
            set
            {
                if (mediaPlayingList.CoverImagePath != value)
                {
                    mediaPlayingList.CoverImagePath = value;
                    OnPropertyChanged(nameof(CoverImagePath));
                }
            }
        }

        public DateTime CreatedTime
        {
            get { return mediaPlayingList.CreatedTime; }
            set
            {
                if (mediaPlayingList.CreatedTime != value)
                {
                    mediaPlayingList.CreatedTime = value;
                    OnPropertyChanged(nameof(CreatedTime));
                }
            }
        }

        public DateTime ModifiedTime
        {
            get { return mediaPlayingList.ModifiedTime; }
            set
            {
                if (mediaPlayingList.ModifiedTime != value)
                {
                    mediaPlayingList.ModifiedTime = value;
                    OnPropertyChanged(nameof(ModifiedTime));
                }
            }
        }

        public ObservableCollection<MediaDesktopItemViewModel> MediaItems
        {
            get { return mediaPlayingList.MediaItems; }
            private set
            {
                if (mediaPlayingList.MediaItems != value)
                {
                    mediaPlayingList.MediaItems = value;
                    OnPropertyChanged(nameof(MediaItems));
                }
            }
        }
        public MediaPlayingListViewModel Self
        {
            get { return this; }
        }
        #endregion




        #region Methods
        public void PlayMediaList()
        {
            if (MediaItems.Any())
            {
                GlobalResources.ViewModelCollection.CurrentPlayingList.Clear();
                foreach(var item in MediaItems)
                {
                    GlobalResources.ViewModelCollection.CurrentPlayingList.Add(item);
                }
                MediaItems.First().MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
            }
        }

        public async Task RemoveMediaList(XamlRoot xamlRoot)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "删除播放列表",
                Content = "确定要移除列表" + this.Title + "吗？",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };
            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (GlobalResources.ViewModelCollection.MediaPlayingListViewModels.Contains(this))
                {
                    GlobalResources.ViewModelCollection.MediaPlayingListViewModels.Remove(this);
                }
            }
        }
        public async void EditMediaList()
        {
            var tempModel = new MediaPlayingListViewModel()
            {
                Title = this.Title,
                Description = this.Description,
                CoverImagePath = this.CoverImagePath
            };

            ContentDialog contentDialog = new ContentDialog()
            {
                Content = new Views.Dialogs.ModifyPlayingListDialogPage() { DataContext = tempModel, Tag = "编辑当前播放列表" },
                PrimaryButtonText = "保存",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = ClientWindow.Instance.Content.XamlRoot
            };

            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                this.Title = tempModel.Title;
                this.CoverImagePath = tempModel.CoverImagePath;
                this.Description = tempModel.Description;
            }

        }

        private void InnerParentFrameNavigateTo(DependencyObject element, Type targetPageType, NavigationTransitionInfo info)
        {
            DependencyObject parent = element;
            do
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent is Frame contentFrame)
                {
                    contentFrame.Navigate(targetPageType, this, info);
                    break;
                }
            }
            while (parent != null);
        }

        public void ParentFrameNavigateToMediaListPage(DependencyObject element)
        {
            InnerParentFrameNavigateTo(element, typeof(MediaPlayingListPage), new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
        }

        public void ParentFrameNavigateToMediaListDetailsPage(DependencyObject element)
        {
            InnerParentFrameNavigateTo(element, typeof(MediaPlayingListDetailsPage), new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        #endregion
        #region Delegate Commands
        public DelegateCommand PlayMediaListCommand { get;  private set; }
        public DelegateCommand RemoveMediaListCommand { get; private set; }
        public DelegateCommand EditMediaListCommand { get; private set; }
        public DelegateCommand ParentFrameNavigateToMediaListDetailsPageCommand { get; private set; }
        public DelegateCommand ParentFrameNavigateToMediaListPageCommand { get; private set; }
        #endregion
        #region Inner Methods
        private void DelegateCommandStartup()
        {
            PlayMediaListCommand = new DelegateCommand((obj) => { PlayMediaList(); });
            RemoveMediaListCommand = new DelegateCommand(async (obj) => { await RemoveMediaList(ClientHostPage.Instance.XamlRoot); });
            EditMediaListCommand = new DelegateCommand((obj) => { EditMediaList(); });
            ParentFrameNavigateToMediaListDetailsPageCommand = new DelegateCommand((obj) => { ParentFrameNavigateToMediaListDetailsPage(obj as DependencyObject); });
            ParentFrameNavigateToMediaListPageCommand = new DelegateCommand(obj => ParentFrameNavigateToMediaListPage(obj as DependencyObject));
        }
        #endregion
        #region Ctor
        public MediaPlayingListViewModel()
        {
            mediaPlayingList = new MediaPlayingList();
            DelegateCommandStartup();
        }
        #endregion
        #region Notify Events&Methods

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
