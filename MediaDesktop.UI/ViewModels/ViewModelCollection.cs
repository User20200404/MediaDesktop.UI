using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDesktop.UI.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using MediaDesktop.UI.Services;
using MediaDesktop.UI.Models;
using MediaDesktop.UI.Views.Pages;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.Input;
using MediaDesktop.UI.Views.Dialogs;
using LibVLCSharp.Shared;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.UI.Xaml.Media.Animation;
using MediaDesktop.UI.Views.UserControls;
using Microsoft.UI.Xaml.Media;

namespace MediaDesktop.UI.ViewModels
{
    public partial class ViewModelCollection : INotifyPropertyChanged
    {

        private MediaDesktopItemViewModel currentDesktopItemViewModel;
        private SettingsItemViewModel settingsItemViewModel;
        private ObservableCollection<MediaDesktopItemViewModel> currentPlayingList;
        private ObservableCollection<MediaDesktopItemViewModel> viewModelItems_Favourite;
        private ObservableCollection<MediaDesktopItemViewModel> viewModelItems_History;
        private ObservableCollection<MediaPlayingListViewModel> mediaPlayingListViewModels;
        private ObservableCollection<UpdateLogViewModel > updateLogViewModels;

        /// <summary>
        /// Do not modify this collection except using provided methods and delegate commands.
        /// </summary>
        public ObservableCollection<MediaDesktopItemViewModel> ViewModelItems { get; private set; }
        public ObservableCollection<SettingsNavigationItemViewModel> SettingsNavigationItemViewModels { get; private set; }
        public ObservableCollection<SettingsNavigationItemViewModel> SettingsNavigationItemViewModels_Bread { get; private set; }
        public IEnumerable<SettingsNavigationItemViewModel> SettingsNavigationItemViewModels_Display { get { return SettingsNavigationItemViewModels.Skip(1); } }
        public MediaDesktopItemViewModelConfig MediaDesktopItemViewModelConfig { get; private set; }
        public MediaPlayingListConfig MediaPlayingListConfig { get; private set; }


        public ObservableCollection<MediaDesktopItemViewModel> CurrentPlayingList
        {
            get { return currentPlayingList; }
            set
            {
                if(currentPlayingList != value)
                {
                    currentPlayingList = value;
                    OnPropertyChanged(nameof(CurrentPlayingList));
                }
            }
        }

        public ObservableCollection<MediaDesktopItemViewModel> ViewModelItems_Favourite
        {
            get { return viewModelItems_Favourite; }
            private set
            {
                if (viewModelItems_Favourite != value)
                {
                    viewModelItems_Favourite = value;
                    OnPropertyChanged(nameof(ViewModelItems_Favourite));
                }
            }
        }
        public ObservableCollection<MediaDesktopItemViewModel> ViewModelItems_History
        {
            get { return viewModelItems_History; }
            private set
            {
                if (viewModelItems_History != value)
                {
                    viewModelItems_History = value;
                    OnPropertyChanged(nameof(ViewModelItems_History));

                }
            }
        }
        public ObservableCollection<UpdateLogViewModel> UpdateLogViewModels
        {
            get { return updateLogViewModels; }
            set 
            {
                if(updateLogViewModels != value)
                {
                    updateLogViewModels = value;
                    OnPropertyChanged(nameof(UpdateLogViewModels));
                }
            }
        }

        public ObservableCollection<MediaPlayingListViewModel> MediaPlayingListViewModels
        {
            get { return mediaPlayingListViewModels; }
            private set
            {
                if(mediaPlayingListViewModels != value)
                {
                    mediaPlayingListViewModels = value;
                    OnPropertyChanged(nameof(MediaPlayingListViewModels));
                }
            }
        }
        public MediaDesktopItemViewModel CurrentDesktopItemViewModel
        {
            get { return currentDesktopItemViewModel; }
            set
            {
                if (currentDesktopItemViewModel != value)
                {
                    currentDesktopItemViewModel = value;
                    OnPropertyChanged(nameof(CurrentDesktopItemViewModel));
                    Debug.WriteLine($"[{DateTime.Now}] CurrentDesktopItemViewModel Changed.");
                }
            }
        }
        public SettingsItemViewModel SettingsItemViewModel
        {
            get { return settingsItemViewModel; }
            set
            {
                if (settingsItemViewModel != value)
                {
                    settingsItemViewModel = value;
                    OnPropertyChanged(nameof(SettingsItemViewModel));
                }
            }
        }


        public ViewModelCollection()
        {
            UpdateLogViewModels = new ObservableCollection<UpdateLogViewModel>();
            UpdateLogViewModel model1 = new UpdateLogViewModel() { Version = "1.0.0.0" };
            model1.Added.Add("实现基本桌面视频播放功能");
            model1.Added.Add("实现自定义添加项目");
            UpdateLogViewModel model2 = new UpdateLogViewModel { Version = "1.0.0.1" };
            model2.Added.Add("实现自定义播放列表功能");
            UpdateLogViewModel model3 = new UpdateLogViewModel { Version = "1.0.0.3" };
            model3.Added.Add("适配了Windows 10和11的系统背景材质");
            model3.Added.Add("实现当前播放项目信息功能");
            model3.Added.Add("实现项目右键菜单功能");
            model3.Modified.Add("修复了一个功能性BUG，该BUG曾导致设置页面导航时程序崩溃");
            UpdateLogViewModel model4 = new UpdateLogViewModel { Version = "1.0.0.6" };
            model4.Modified.Add("修复了一个功能性BUG，该BUG曾导致快速切换适配时发生线程死锁");
            model4.Modified.Add("修复了一个功能性BUG，该BUG曾导致切换媒体文件时自适应拉伸失效");
            model4.Modified.Add("修复了一个功能性BUG，该BUG曾导致切换媒体文件时弹出LibVLC默认输出窗口");
            model4.Modified.Add("修复了一个功能性BUG，该BUG曾导致编辑项目在程序重启前不生效");
            UpdateLogViewModel model5 = new UpdateLogViewModel { Version = "1.1.0.0" };
            model5.Added.Add("加入更新日志功能，该功能在设置->关于中启用");
            model5.Modified.Add("修复了一个内部BUG，该BUG曾导致视图模型多次通知集合变更");
            model5.Modified.Add("修复了一个功能性BUG，该BUG曾导致设置页面中的项目不能显示垂直滚动条");
            UpdateLogViewModel model6 = new UpdateLogViewModel { Version = "1.1.0.1" };
            model6.Added.Add("加入弹出消息功能，现在可以在进行部分操作或程序发生异常时看到这个弹出消息框。");
            model6.Added.Add("加入隐藏播放控件功能");
            UpdateLogViewModel model7 = new UpdateLogViewModel { Version = "1.1.0.2" };
            model7.Added.Add("加入了文件拖拽功能，现在可以将媒体文件拖入库页面来快捷添加项目。");
            model7.Added.Add("加入了隐藏窗口功能，现在可以通过播放控件的折叠菜单来隐藏窗口。");
            model7.Modified.Add("修复了一个功能性BUG，该BUG曾导致删除项目时不会将收藏页面中项目一并移除。");
            model7.Modified.Add("更新了设置页面中的部分UI。");
            UpdateLogViewModel model8 = new UpdateLogViewModel { Version = "1.1.1.0" };
            model8.Added.Add("加入了帧画面提取功能，现在可以在编辑项目页面中通过提取帧来设置封面。");
            model8.Added.Add("完善了库项目缩放功能，现在可以通过CTRL+鼠标滚轮来设置大多数元素的大小。");
            model8.Added.Add("加入了播放下一个功能，现在可以通过右键项目来将其设置为下一个播放项。");
            model8.Modified.Add("优化了库项目加载的方法，现在程序启动的响应速度得到一定提升。");
            model8.Modified.Add("优化了更新日志的显示方法，现在不会在没有相关内容时追加换行符。");
            model8.Modified.Add("修复了一个功能性BUG，该BUG曾导致在双击菜单项时，程序因重复打开ContentDialog而崩溃。");
            model8.Modified.Add("修复了一个内部BUG，该BUG曾导致同一种ContentDialog被重复显示。");
            model8.Modified.Add("修复了一个功能性BUG，该BUG曾导致在修改背景材质时，效果不会即时应用。");
            UpdateLogViewModels.Add(model1);
            UpdateLogViewModels.Add(model2);
            UpdateLogViewModels.Add(model3);
            UpdateLogViewModels.Add(model4);
            UpdateLogViewModels.Add(model5);
            updateLogViewModels.Add(model6);
            updateLogViewModels.Add(model7);
            updateLogViewModels.Add(model8);
            string s = JsonSerializer.Serialize(UpdateLogViewModels, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText("C:\\Users\\31129\\Desktop\\UpdateLogs.txt", s);

            ViewModelItems = new ObservableCollection<MediaDesktopItemViewModel>();
            MediaPlayingListViewModels = new ObservableCollection<MediaPlayingListViewModel>();
            CurrentPlayingList = new ObservableCollection<MediaDesktopItemViewModel>();
            MediaDesktopItemViewModelConfig = new MediaDesktopItemViewModelConfig(this);
            MediaPlayingListConfig = new MediaPlayingListConfig(this);

            SettingsItemViewModel = new SettingsItemViewModel();
            InitSettingsNavigationViewItems();
            InitUpdateLogItems();
            EventStartup();
            DelegateCommandStartup();
        }

     


        #region Inner Methods
        private void InitUpdateLogItems()
        {
            UpdateLogViewModels = ViewModelHelper.GetUpdateLogViewModelCollectionFromJSON(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\logs\\UpdateLogs.txt");
        }

        private void InitSettingsNavigationViewItems()
        {
            SettingsNavigationItemViewModels = new ObservableCollection<SettingsNavigationItemViewModel>()
            {
               new SettingsNavigationItemViewModel() { Icon = "\xF8B0", Title = "设置", Introduction = "主页面",PageName = "设置",PageType = typeof(SettingsHostPage) },
               new SettingsNavigationItemViewModel() { Icon = "\xEA40", Title = "存储选项", Introduction = "配置文件位置",PageName = "存储选项",PageType = typeof(SettingsPage_Storage) },
               new SettingsNavigationItemViewModel() {Icon = "\xEA69",Title = "播放选项",Introduction="LibVLC组件配置",PageName="播放选项",PageType=typeof(SettingsPage_Player)},
               new SettingsNavigationItemViewModel() { Icon = "\xE771", Title = "个性化", Introduction = "改变窗口外观",PageName = "个性化",PageType = typeof(SettingsPage_Personalize) },
               new SettingsNavigationItemViewModel() { Icon = "\xE946", Title = "关于", Introduction = "作者、软件版本",PageName = "关于",PageType = typeof(SettingsPage_About) }
            };

            SettingsNavigationItemViewModels_Bread = new ObservableCollection<SettingsNavigationItemViewModel>();
        }

        private void DelegateCommandStartup()
        {
            PlayNextCommand = new DelegateCommand((obj) => { PlayNext(); });
            PlayLastCommand = new DelegateCommand((obj) => { PlayLast(); });
            AddPlayingListViewModelCommand = new DelegateCommand(async (obj) => { await AddPlayingListViewModel(Views.Pages.ClientHostPage.Instance.XamlRoot); });
            AddViewModelCommand = new DelegateCommand((obj) => { AddViewModel(Views.Pages.ClientHostPage.Instance.XamlRoot); });
            EditViewModelCommand = new DelegateCommand((obj) => { EditViewModel(Views.Pages.ClientHostPage.Instance.XamlRoot, obj as MediaDesktopItemViewModel); });
            RemoveViewModelCommand = new DelegateCommand((obj) => { RemoveViewModel(Views.Pages.ClientHostPage.Instance.XamlRoot, obj as MediaDesktopItemViewModel); });
            ResetHistoryCommand = new DelegateCommand((obj) => { ResetHistory(Views.Pages.ClientHostPage.Instance.XamlRoot); });

        }

        private void EventStartup()
        {
            GlobalResources.InitializeCompleted += GlobalResources_InitializeCompleted;
            GlobalResources.MediaDesktopPlayer.MediaPlayerEndReached += MediaDesktopPlayer_MediaPlayerEndReached;
            //SettingsNavigationItemViewModels_Bread.CollectionChanged += SettingsNavigationItemViewModels_Bread_CollectionChanged;
            ViewModelItems.CollectionChanged += ViewModelItems_CollectionChanged;
            CurrentPlayingList.CollectionChanged += CurrentPlayingList_CollectionChanged;
        }

    

        //private void SettingsNavigationItemViewModels_Bread_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    ClientHostPage.Instance.DispatcherQueue.TryEnqueue(() => { OnPropertyChanged(nameof(SettingsNavigationItemViewModels_Bread)); });
        //}

        private void MediaDesktopPlayer_MediaPlayerEndReached(object sender, EventArgs e)
        { 
            PlayBackMode mode = SettingsItemViewModel.PlayBackMode;

            switch (mode)
            {
                case PlayBackMode.Shuffle:
                    PlayNext();
                    break;
                case PlayBackMode.RepeatOne:
                    PlayRepeatOne();
                    break;
                default:
                    break;
            }
        }
        private void PlayRepeatOne()
        {
            Task.Run(() =>
            {
                CurrentDesktopItemViewModel.MediaItemViewModel.RuntimeDataSet.MediaPlayer.Media = CurrentDesktopItemViewModel.MediaItemViewModel.Media;
                CurrentDesktopItemViewModel.MediaItemViewModel.RuntimeDataSet.MediaPlayer.Play();
            });
        }

        private void PlayNext()
        {
            if (!CurrentPlayingList.Any())
                return;

            if (CurrentPlayingList.First() == CurrentPlayingList.Last())
            {
                PlayRepeatOne();
                return;
            }

            Task.Run(() =>
            {
                int indexToPlay = GetNextIndexToPlayAt();
                CurrentPlayingList[indexToPlay].MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
                GlobalResources.MediaDesktopPlayer.Play();
            });
        }

        public int GetNextIndexToPlayAt()
        {
            int currentIndex = CurrentPlayingList.IndexOf(CurrentDesktopItemViewModel);
            if(CurrentPlayingList.Last() == CurrentDesktopItemViewModel)
            {
                return 0;
            }
            else
            {
                return currentIndex + 1;
            }
        }

        private void PlayLast()
        {
            if (!CurrentPlayingList.Any())
                return;
            if (CurrentPlayingList.First() == CurrentPlayingList.Last())
            {
                PlayRepeatOne();
                return;
            }

            Task.Run(() =>
            {
                int index = CurrentPlayingList.IndexOf(CurrentDesktopItemViewModel);
                if (CurrentPlayingList.First() == CurrentDesktopItemViewModel)
                {
                    //GlobalResources.MediaDesktopPlayer.MediaPlayer = CurrentPlayingList.First().MediaItemViewModel.RuntimeDataSet.MediaPlayer;
                    CurrentPlayingList.Last().MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);

                    if (CurrentPlayingList.First() == CurrentPlayingList.Last())
                        GlobalResources.MediaDesktopPlayer.Stop();
                }
                else
                {
                    // GlobalResources.MediaDesktopPlayer.MediaPlayer = CurrentPlayingList[index + 1].MediaItemViewModel.RuntimeDataSet.MediaPlayer;
                    CurrentPlayingList[index - 1].MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
                }

                GlobalResources.MediaDesktopPlayer.Play();
            });
        }

        private void CurrentPlayingList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.OldItems != null)
            {
                bool isPlaying = CurrentDesktopItemViewModel.MediaItemViewModel.RuntimeDataSet.IsMediaPlaying;
                if (e.OldItems.Contains(CurrentDesktopItemViewModel))
                {
                    CurrentDesktopItemViewModel.MediaItemViewModel.StopMedia(GlobalResources.MediaDesktopPlayer);
                    if (CurrentPlayingList.Count > e.OldStartingIndex)
                    {
                        CurrentDesktopItemViewModel = CurrentPlayingList[e.OldStartingIndex];
                    }
                    else
                    {
                        CurrentDesktopItemViewModel = CurrentPlayingList.LastOrDefault();
                    }

                    if(isPlaying && CurrentDesktopItemViewModel!=null)
                    {
                        CurrentDesktopItemViewModel.MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
                    }
                }
            }

            if(e.NewItems!=null)
            {
                if(CurrentDesktopItemViewModel == null)
                {
                    CurrentDesktopItemViewModel = e.NewItems[0] as MediaDesktopItemViewModel;
                }
            }
        }


        private void ViewModelItems_CollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ViewModelItems));
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var model = item as MediaDesktopItemViewModel;
                    model.MediaItemViewModel.RuntimeDataSet.PropertyChanged += RuntimeDataViewModel_PropertyChanged;
                    model.HistoryLevelResetRequired += DesktopItemViewModel_HistoryLevelResetRequired;
                    model.PropertyChanged += DesktopItemViewModel_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var model = item as MediaDesktopItemViewModel;
                    model.MediaItemViewModel.RuntimeDataSet.PropertyChanged -= RuntimeDataViewModel_PropertyChanged;
                    model.HistoryLevelResetRequired -= DesktopItemViewModel_HistoryLevelResetRequired;
                    model.PropertyChanged -= DesktopItemViewModel_PropertyChanged;
                    UpdateHistoryLevel(model, HistoryLevelUpdateReason.ItemRemoved);
                }
                UpdateFavouriteCollection(false);
            }
            MediaDesktopItemViewModelConfig.Save();
        }

        private void DesktopItemViewModel_HistoryLevelResetRequired(object sender, EventArgs e)
        {
            UpdateHistoryLevel(sender as MediaDesktopItemViewModel, HistoryLevelUpdateReason.HistoryLevelResetRequired);
        }

        /// <summary>
        /// Monitors and saves the changes of <see cref="ViewModelItems"/>
        /// </summary>
        /// <param name="sender">The instance of <see cref="MediaDesktopItemViewModel"/></param>
        /// <param name="e"></param>
        private void DesktopItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MediaDesktopItemViewModelConfig.Save();
            MediaDesktopItemViewModel model = sender as MediaDesktopItemViewModel; 
            switch(e.PropertyName)
            {
                case nameof(model.IsFavourite): //If the varied property name is "IsFavourite", updates favourite items collection.
                    if(model.IsFavourite)
                    {
                        ViewModelItems_Favourite.Add(model);
                    }
                    else
                    {
                        ViewModelItems_Favourite.Remove(model);
                    }
                    break;
            }
        }

        private void RuntimeDataViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsMediaPlaying")
            {
                if ((sender as MediaItemViewModel.RuntimeData).IsMediaPlaying) //Media is being played.
                {
                    CurrentDesktopItemViewModel = GetCurrentPlayingModel();
                    UpdateHistoryLevel(CurrentDesktopItemViewModel, HistoryLevelUpdateReason.ItemPlayed);
                    //OnPropertyChanged(nameof(CurrentDesktopItemViewModel));
                }
            }
        }


        private void GlobalResources_InitializeCompleted(object sender, EventArgs e)
        {
                MediaDesktopItemViewModelConfig.InitViewModel(); //reads configs and restores the items last time.
                MediaPlayingListConfig.InitViewModel();
                InitFavouriteCollection();
                InitHistoryCollection();
        }


        /// <summary>
        /// Gets <see cref="MediaDesktopItemViewModel"/> that's currently being played, 
        /// </summary>
        /// <returns></returns>
        private MediaDesktopItemViewModel GetCurrentPlayingModel()
        {
            var collection = ViewModelItems.Where(model => model.MediaItemViewModel.RuntimeDataSet.IsMediaPlaying is true);
            if (collection.Any())
            {
                //When switching playing items, both the old one and the new one's RuntimeDataSet.IsMediaPlaying will be true.
                var collection_NotCurrent = collection.Where(model => model != CurrentDesktopItemViewModel); 
                if(collection_NotCurrent.Any())
                {
                    return collection_NotCurrent.First();
                }
                else
                {
                    return collection.First();
                }
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Clears all items in <seealso cref="ViewModelItems_Favourite"/> and reload it from <see cref="ViewModelItems"/>.
        /// </summary>
        private void InitFavouriteCollection()
        {
            if (ViewModelItems_Favourite is null)
                ViewModelItems_Favourite = new ObservableCollection<MediaDesktopItemViewModel>(ViewModelItems.Where(item => item.IsFavourite is true)); //creates favourite item collection from basic item collection.
            else
            {
                var temp = new ObservableCollection<MediaDesktopItemViewModel>(ViewModelItems.Where(item => item.IsFavourite is true));
                ViewModelItems_Favourite.Clear();
                foreach (var item in temp)
                    ViewModelItems_Favourite.Add(item);
            }
        }

        /// <summary>
        /// Initializes history items sorted based on history level.
        /// </summary>
        private void InitHistoryCollection()
        {
            List<MediaDesktopItemViewModel> orderList = ViewModelItems.Where(i => i.HistoryLevel != -1).ToList();
            orderList.Sort((m1, m2) =>
            {
                if (m1.HistoryLevel > m2.HistoryLevel)
                    return 1;
                else return -1;
            });
            ViewModelItems_History = new ObservableCollection<MediaDesktopItemViewModel>(orderList);
        }

        /// <summary>
        /// Updates favourite items. Should be called when removing items from <seealso cref="ViewModelItems"/>
        /// </summary>
        /// <param name="rebuild">To clear all items and reload from <seealso cref="ViewModelItems"/>, set to true. To only remove the unfound items in <seealso cref="ViewModelItems"/>, set to false.</param>
        private void UpdateFavouriteCollection(bool reload = false)
        {
            if (reload)
            {
                InitFavouriteCollection();
            }
            else
            {
                List<MediaDesktopItemViewModel> modelsToRemove = new List<MediaDesktopItemViewModel>();
                foreach (var item in ViewModelItems_Favourite.Where(i => !ViewModelItems.Contains(i)))
                {
                    modelsToRemove.Add(item);
                }

                foreach (var item in modelsToRemove)
                {
                    ViewModelItems_Favourite.Remove(item);
                }
            }
        }

        private void UpdateHistoryLevel(MediaDesktopItemViewModel model, HistoryLevelUpdateReason reason)
        {
            switch (reason)
            {
                case HistoryLevelUpdateReason.None:
                    break;
                case HistoryLevelUpdateReason.ItemRemoved:
                    UpdateHistoryLevel_ItemRemoved(model);
                    break;
                case HistoryLevelUpdateReason.ItemPlayed:
                    UpdateHistoryLevel_ItemPlayed(model);
                    break;
                case HistoryLevelUpdateReason.HistoryLevelResetRequired:
                    UpdateHistoryLevel_ResetRequired(model);
                    break;
                default:
                    throw new ArgumentException("History Update Reason should not be None(0).");
            }
        }

        private void UpdateHistoryLevel_ItemRemoved(MediaDesktopItemViewModel model)
        {
            #region Model Data Handling
            int oldLevel = model.HistoryLevel;
            if (oldLevel != -1)
            {
                foreach (var i in ViewModelItems)
                {
                    if (i.HistoryLevel > oldLevel)
                    {
                        i.HistoryLevel--;
                    }
                }
            }
            #endregion
            #region Collection Data Handling
            if (ViewModelItems_History.Contains(model))
            {
                ViewModelItems_History.Remove(model);
            }
            #endregion
        }

        private void UpdateHistoryLevel_ItemPlayed(MediaDesktopItemViewModel model)
        {
            #region Model Data Handling
            int oldLevel = model.HistoryLevel;
            if (oldLevel == -1) //First Play
            {
                foreach (var i in ViewModelItems)
                {
                    if (i.HistoryLevel != -1)
                    {
                        i.HistoryLevel++;
                    }
                }
            }
            else //Not First Play
            {
                foreach (var i in ViewModelItems)
                {
                    if (i.HistoryLevel < oldLevel && i.HistoryLevel != -1)
                    {
                        i.HistoryLevel++;
                    }
                }
            }
            CurrentDesktopItemViewModel.HistoryLevel = 0;
            #endregion

            #region Collection Data Handling
            if (ViewModelItems_History.FirstOrDefault() != model)
            {
                if (ViewModelItems_History.Contains(model))
                {
                    ViewModelItems_History.Remove(model);
                }
                ViewModelItems_History.Insert(0, model);
            }
            #endregion
        }

        private void UpdateHistoryLevel_ResetRequired(MediaDesktopItemViewModel model)
        {
            #region Data Handling
            int oldLevel = model.HistoryLevel;
            model.HistoryLevel = -1;

            foreach (var i in ViewModelItems)
            {
                if (i.HistoryLevel > oldLevel)
                {
                    i.HistoryLevel--;
                }
            }
            #endregion
            #region Collection Data Handling
            if(ViewModelItems_History.Contains(model))
            {
                ViewModelItems_History.Remove(model);
            }
            #endregion
        }

        private enum HistoryLevelUpdateReason
        {
            None = 0,
            ItemRemoved = 1,
            ItemPlayed = 2,
            HistoryLevelResetRequired = 3
        }
        #endregion

        #region Methods
        public async Task AddPlayingListViewModel(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            var model = new MediaPlayingListViewModel();
            
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = new ModifyPlayingListDialogPage() { DataContext = model, Tag = "新建播放列表" },
                PrimaryButtonText = "新建",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                MediaPlayingListViewModels.Add(model); 
            }

        }

        public async void AddViewModel(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            var model = new MediaDesktopItemViewModel();

            NotifyButtonClickContentDialog contentDialog = new NotifyButtonClickContentDialog()
            {
                Content = new ModifyItemDialogPage(model) { DataContext = model, Tag = ModifyItemOperation.CreateNew },
                PrimaryButtonText = "添加",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                IsPrimaryButtonEnabled = false,
                XamlRoot = xamlRoot
            };

            contentDialog.PromisedShowDeferral();
            //if (result == ContentDialogResult.Primary)
            //{
            //    if (!File.Exists(model.MediaPath))
            //    {
            //        return;
            //    }

            //    ViewModelItems.Add(model);
            //}
        }

        public void EditViewModel(Microsoft.UI.Xaml.XamlRoot xamlRoot, MediaDesktopItemViewModel model)
        {
            MediaDesktopItemViewModel tempModel = new MediaDesktopItemViewModel()
            {
                Title = model.Title,
                SubTitle = model.SubTitle,
                MediaPath = model.MediaPath,
                ImagePath = model.ImagePath
            };

            NotifyButtonClickContentDialog contentDialog = new NotifyButtonClickContentDialog()
            {
                Content = new ModifyItemDialogPage(model) { DataContext = tempModel, Tag = ModifyItemOperation.ModifyExisting},
                PrimaryButtonText = "保存",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                IsPrimaryButtonEnabled=false,
                XamlRoot = xamlRoot
            };

            contentDialog.PromisedShowDeferral();
            //if (result == ContentDialogResult.Primary)
            //{
            //    model.Title = tempModel.Title;
            //    model.SubTitle = tempModel.SubTitle;
            //    model.MediaPath = tempModel.MediaPath;
            //    model.ImagePath = tempModel.ImagePath;
            //}
        }

        public async void RemoveViewModel(XamlRoot xamlRoot, MediaDesktopItemViewModel model)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "删除项目",
                Content = "确定要移除项目" + model.Title + "吗？",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };
            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModelItems.Remove(model);
            }
        }

        public async void ResetHistory(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "清空历史记录",
                Content = "所有项目的播放历史都将被重置，且无法撤销此操作。",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };
            var result = await contentDialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                foreach(var i in ViewModelItems_History)
                {
                    i.HistoryLevel = -1;
                }
                ViewModelItems_History.Clear();
            }
        }

        #endregion

        #region Delegate Commands
        public DelegateCommand PlayNextCommand { get; private set; }
        public DelegateCommand PlayLastCommand { get; private set; }
        public DelegateCommand AddPlayingListViewModelCommand { get; private set; }
        public DelegateCommand AddViewModelCommand { get; private set; }
        public DelegateCommand EditViewModelCommand { get; private set; }
        public DelegateCommand RemoveViewModelCommand { get; private set; }
        public DelegateCommand ResetHistoryCommand { get; private set; }

        #endregion

        #region Notify Event&Method

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
