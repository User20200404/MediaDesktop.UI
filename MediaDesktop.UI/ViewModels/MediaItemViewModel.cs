using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDesktop.UI.Models;
using LibVLCSharp.Shared;
using System.Drawing;
using MediaDesktop.UI.Services;
using Microsoft.UI.Xaml.Controls;
using System.Threading;
using Microsoft.UI.Xaml;
using System.Drawing.Text;
using System.IO;
using MediaDesktop.UI.Helpers;

namespace MediaDesktop.UI.ViewModels
{
    public class MediaItemViewModel : INotifyPropertyChanged
    {
        #region Raw Data Source
        private MediaItem mediaItem;
        private LibVLC libVLC;
        #endregion

        #region Raw Data Encapsulation
        public string MediaPath
        {
            get { return mediaItem.MediaPath; }
            set
            {
                if (mediaItem.MediaPath != value)
                {
                    mediaItem.MediaPath = value;
                    LoadMedia(GlobalResources.LibVLC);
                    OnPropertyChanged(nameof(MediaPath));
                }
            }
        }
        /// <summary>
        /// Gets the media file size in bytes.
        /// </summary>
        public long MediaSize
        {
            get { return new System.IO.FileInfo(MediaPath).Length; }
        }

        public bool IsMediaLoaded
        {
            get { return mediaItem.IsMediaLoaded; }
        }

        public Media Media
        {
            get { return mediaItem.Media; }
            private set
            {
                if (mediaItem.Media != value)
                {
                    mediaItem.Media = value;
                    OnPropertyChanged(nameof(Media));
                }
            }
        }


        #endregion

        #region Runtime Data Encapsulation
        public RuntimeData RuntimeDataSet { get; private set; }
        #endregion

        #region Methods

        public void LoadMedia(LibVLC libVLC)
        {
            if (!File.Exists(MediaPath))
                return;

            this.libVLC = libVLC;
            int fileCaching = 500;
            if (GlobalResources.IsInitialized)
                fileCaching = GlobalResources.ViewModelCollection.SettingsItemViewModel.FileCaching;
            Media = new Media(libVLC, new Uri(MediaPath), "--file-caching=" + fileCaching.ToString());
            if (RuntimeDataSet != null)
            {
                RuntimeDataSet.ReLoad(mediaItem);
            }
            else
            {
                RuntimeDataSet = new RuntimeData(mediaItem);
            }
        }

        public void UpdateMediaFileCaching(int fileCaching)
        {
            Media.AddOption("--file-caching=" + fileCaching.ToString());
        }

        public void PlayMedia(MediaDesktopPlayer player)
        {
            RuntimeDataSet.PlayMedia(player);
        }

        public void InsertMedia()
        {
            RuntimeDataSet.InsertMedia();
        }

        public void PauseMedia(MediaDesktopPlayer player)
        {
            RuntimeDataSet.PauseMedia(player);
        }

        public void TogglePlayStatus(MediaDesktopPlayer player)
        {
            if(RuntimeDataSet.MediaPlayer.IsPlaying)
            {
                PauseMedia(player);
            }
            else
            {
                PlayMedia(player);
            }
        }

        public void ToggleMediaStatusTo(MediaDesktopPlayer player, ToggleMediaStatusAction action)
        {
            switch (action)
            {
                case ToggleMediaStatusAction.Pause:
                    PauseMedia(player);
                    break;
                case ToggleMediaStatusAction.Play:
                    PlayMedia(player);
                    break;
                case ToggleMediaStatusAction.Stop:
                    StopMedia(player);
                    break;
            }
        }

        public void StopMedia(MediaDesktopPlayer player)
        {
            RuntimeDataSet.StopMedia(player);
        }

        public async void ShowMediaInfoDialog(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                PrimaryButtonText = "关闭",
                Content = new Views.Dialogs.MediaInfoDialogPage() { DataContext = this },
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            await contentDialog.ShowAsync();
        }

        /// <summary>
        /// Clears all items in the current playing list, and refill it with <paramref name="list"/>. The first item is this viewmodel.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="list">The list to play.</param>
        /// <exception cref="Exception">Throws when target list does not contain current viewmodel.</exception>
        public void PlayWithMediaList(MediaDesktopPlayer player,MediaPlayingListViewModel list)
        {
            var desktopItem = list.MediaItems.FirstOrDefault(i => i.MediaItemViewModel == this);
            if(desktopItem == default(MediaDesktopItemViewModel))
            {
                throw new Exception(nameof(PlayWithMediaList) + ": This method could only be invoked where current MediaItemViewModel is included in the given list.");
            }
            else
            {
                GlobalResources.ViewModelCollection.CurrentPlayingList.Clear();
                int index = list.MediaItems.IndexOf(desktopItem);
                
                //Refill CurrentPlayingList with the items in list, where ordering begins at desktopItem.
                foreach (var item in list.MediaItems.Where(i => list.MediaItems.IndexOf(i) >= index))
                {
                    GlobalResources.ViewModelCollection.CurrentPlayingList.Add(item);
                }

                foreach (var item in list.MediaItems.Where(i => list.MediaItems.IndexOf(i) < index))
                {
                    GlobalResources.ViewModelCollection.CurrentPlayingList.Add(item);
                }

                desktopItem.MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
            }
        }

        public void BrowseMediaFileInExplorer()
        {
            WinUI3Helper.SelectFileInExplorer(MediaPath);
        }

        public async Task RemoveFromMediaPlayingList(IList<MediaDesktopItemViewModel> list, XamlRoot xamlRoot)
        {
            var desktopItemViewModel = list.FirstOrDefault(i => i.MediaItemViewModel == this);
            if (desktopItemViewModel != default(MediaDesktopItemViewModel))
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "从播放列表中移除项目",
                    Content = "确定要移除项目" + desktopItemViewModel.Title + "吗？\n\n这不会从库中删除此项目。",
                    PrimaryButtonText = "确定",
                    SecondaryButtonText = "取消",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = xamlRoot
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                    list.Remove(desktopItemViewModel);
            }
        }
        #endregion

        #region DelegateCommands
        public DelegateCommand LoadMediaCommand { get; set; }
        public DelegateCommand PlayMediaCommand { get; set; }
        public DelegateCommand PauseMediaCommand { get; set; }
        public DelegateCommand StopMediaCommand { get; set; }
        public DelegateCommand PlayWithMediaListCommand { get; set; }
        public DelegateCommand TogglePlayStatusCommand { get; set; }
        public DelegateCommand ToggleMediaStatusToCommand { get; set; }
        public DelegateCommand ShowMediaInfoDialogCommand { get; set; }
        public DelegateCommand SelectFileInExplorerCommand { get; set; }
        public DelegateCommand RemoveFromMediaPlayingListCommand { get; set; }
        public DelegateCommand InsertMediaCommand { get; set; }

        #endregion

        #region Ctor
        /// <summary>
        /// Ctor.
        /// </summary>
        public MediaItemViewModel(MediaItem mediaItem)
        {
            this.mediaItem = mediaItem;
            LoadMediaCommand = new DelegateCommand((obj) => { LoadMedia(obj as LibVLC); });
            PlayMediaCommand = new DelegateCommand((obj) => { PlayMedia(obj as MediaDesktopPlayer); });
            PauseMediaCommand = new DelegateCommand((obj) => { PauseMedia(obj as MediaDesktopPlayer); });
            StopMediaCommand = new DelegateCommand((obj) => { StopMedia(obj as MediaDesktopPlayer); });
            PlayWithMediaListCommand = new DelegateCommand((obj) => { PlayWithMediaList(GlobalResources.MediaDesktopPlayer, obj as MediaPlayingListViewModel); });
            TogglePlayStatusCommand = new DelegateCommand((obj) => { TogglePlayStatus(obj as MediaDesktopPlayer); });
            ToggleMediaStatusToCommand = new DelegateCommand((obj) => { var data = (ToggleMediaStatusData)obj; ToggleMediaStatusTo(data.Player, data.Action); });
            ShowMediaInfoDialogCommand = new DelegateCommand((obj) => { ShowMediaInfoDialog(Views.Pages.ClientHostPage.Instance.XamlRoot); });
            SelectFileInExplorerCommand = new DelegateCommand((obj) => { BrowseMediaFileInExplorer(); });
            RemoveFromMediaPlayingListCommand = new DelegateCommand(async (obj) => { await RemoveFromMediaPlayingList(obj as IList<MediaDesktopItemViewModel>, Views.Pages.ClientHostPage.Instance.XamlRoot); });
            InsertMediaCommand = new DelegateCommand(obj => InsertMedia());
        }
        #endregion

        #region Notify Event&Method

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// Represents and places the data produced on run time.
        /// </summary>
        public class RuntimeData : INotifyPropertyChanged
        {
            private MediaStrechMode strechMode;
            private MediaPlayer mediaPlayer;
            private MediaDesktopPlayer relatedMediaDesktopPlayer;
            private bool isMediaDesktopPlayerChangeRequestCompleted;
            /// <summary>
            /// Used to get a media's Info.
            /// </summary>
            public MediaPlayer MediaPlayer
            {
                get { return mediaPlayer; }
                set
                {
                    if (mediaPlayer != value)
                    {
                        mediaPlayer = value;
                        OnPropertyChanged(nameof(MediaPlayer));
                    }
                }
            }
            private MediaItem MediaItem { get; set; }
            private MediaDesktopPlayer RelatedMediaDesktopPlayer
            {
                get => relatedMediaDesktopPlayer;
                set
                {
                    if(relatedMediaDesktopPlayer != value)
                    {
                        relatedMediaDesktopPlayer = value;
                        IsMediaDesktopPlayerChangeRequsetCompleted = false;
                        MediaDesktopPlayerEventStartup(value);
                    }
                }
            }
            private Media Media { get { return MediaItem.Media; } }

            private void EventStartup()
            {
                MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                MediaPlayer.Media.StateChanged += Media_StateChanged;
            }

            private void EventLogOff()
            {
                MediaPlayer.PositionChanged -= MediaPlayer_PositionChanged;
                MediaPlayer.Media.StateChanged -= Media_StateChanged;
            }

            private void Initialize(MediaItem mediaItem)
            {
                MediaItem = mediaItem;
                MediaPlayer = new MediaPlayer(Media);
                Media.Parse(MediaParseOptions.ParseLocal).Wait();
                EventStartup();
            }

            public RuntimeData(MediaItem mediaItem)
            {
                Initialize(mediaItem);
            }
            
            ~RuntimeData()
            {
                EventLogOff();
            }

            #region Methods
            private void MediaDesktopPlayerEventStartup(MediaDesktopPlayer player)
            {
                if (player is not null)
                {
                    player.MediaPlayerChangeRequestCompleted += MediaDesktopPlayer_MediaPlayerChangeRequestCompleted;
                    player.MediaPlayerChanging += MediaDesktopPlayer_MediaPlayerChanging;
                }
            }

            private void MediaDesktopPlayer_MediaPlayerChanging(object sender, MediaPlayerChangeEventArgs args)
            {
                MediaDesktopPlayerEventLogOff((MediaDesktopPlayer)sender);
            }

            private void MediaDesktopPlayer_MediaPlayerChangeRequestCompleted(object sender, MediaPlayerChangeEventArgs args)
            {
                IsMediaDesktopPlayerChangeRequsetCompleted = true;
            }

            private void MediaDesktopPlayerEventLogOff(MediaDesktopPlayer player)
            {
                if(player is not null)
                {
                    player.MediaPlayerChangeRequestCompleted -= MediaDesktopPlayer_MediaPlayerChangeRequestCompleted;
                }
            }
            private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
            {
                OnPropertyChanged(nameof(MediaCurrentTime));
                OnPropertyChanged(nameof(MediaPlayedProgress));
                OnPropertyChanged(nameof(MediaKbps));
            }
            private void Media_StateChanged(object sender, MediaStateChangedEventArgs e)
            {
                OnPropertyChanged(nameof(IsMediaPlaying));
            }

            private void ApplyStrechMode(MediaStrechMode mode)
            {
                Size scrSize = WinUI3Helper.GetScreenResolution();
                switch (mode)
                {
                    case MediaStrechMode.Uniform:
                        {
                            MediaPlayer.Scale = 1f;
                            MediaPlayer.AspectRatio = null;
                            break;
                        }
                    case MediaStrechMode.Strech:
                        {
                            MediaPlayer.Scale = 1f;
                            MediaPlayer.AspectRatio = scrSize.Width.ToString() + ":" + scrSize.ToString();
                            break;
                        }
                    case MediaStrechMode.UniformToFill:
                        {
                            MediaPlayer.AspectRatio = null;

                            Size mediaResolution = MediaResolution;
                            float scrRatio = (float)scrSize.Width / (float)scrSize.Height;
                            float mediaRatio = (float)mediaResolution.Width / (float)mediaResolution.Height;

                            float scaleRatio;
                            if (scrRatio <= mediaRatio)
                            {
                                scaleRatio = (float)scrSize.Height / (float)MediaResolution.Height;
                            }
                            else
                            {
                                scaleRatio = (float)scrSize.Width / (float)MediaResolution.Width;
                            }

                            MediaPlayer.Scale = scaleRatio;
                            break;
                        }
                }
            }

            public void PlayMedia(MediaDesktopPlayer player)
            {
                var parentDesktopViewModel = this.GetParentDesktopViewModel();
                if (parentDesktopViewModel != null)
                {
                    if (!ViewModelHelper.CurrentPlayingListContains(parentDesktopViewModel))
                    {
                        GlobalResources.ViewModelCollection.CurrentPlayingList.Add(parentDesktopViewModel);
                    }
                    player.MediaPlayer = MediaPlayer;

                    if (player.MediaPlayer == MediaPlayer) //if change success
                    {
                        RelatedMediaDesktopPlayer = player;
                        player.MediaPlayer.EnableHardwareDecoding = GlobalResources.ViewModelCollection.SettingsItemViewModel.EnableHardwareDecoding;
                        player.Play();
                        GlobalResources.PushForegroundMessage($"正在播放：{parentDesktopViewModel.Title}",TimeSpan.FromMilliseconds(300));
                    }
                }
            }

            public void InsertMedia(MediaDesktopPlayer player)
            {
                var parentDesktopItemViewModel = this.GetParentDesktopViewModel();
                if (parentDesktopItemViewModel != null)
                {
                    int currentPlayingIndex = GlobalResources.ViewModelCollection.CurrentPlayingList.
                             IndexOf(GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel);
                    if (!ViewModelHelper.CurrentPlayingListContains(parentDesktopItemViewModel))
                    {
                        if (currentPlayingIndex == -1) //the list previously does not contain any element, so we just play it.
                            PlayMedia(player);
                        else GlobalResources.ViewModelCollection.CurrentPlayingList.Insert(currentPlayingIndex + 1, parentDesktopItemViewModel);
                    }
                    else
                    {
                        //Current playing list contains this viewmodel.
                        int thisIndex = GlobalResources.ViewModelCollection.CurrentPlayingList.
                            IndexOf(parentDesktopItemViewModel);
                        if (thisIndex == currentPlayingIndex)
                        {
                            GlobalResources.PushForegroundMessage("项目已经在播放中", TimeSpan.FromMilliseconds(600));
                            return; 
                        }

                        int nextIndex = GlobalResources.ViewModelCollection.GetNextIndexToPlayAt();
                        GlobalResources.ViewModelCollection.CurrentPlayingList.Move(thisIndex, nextIndex);
                    }
                    GlobalResources.PushForegroundMessage($"将 {parentDesktopItemViewModel.Title} 作为下一个播放项",TimeSpan.FromMilliseconds(900));
                }
            }
            public void InsertMedia()
            {
                InsertMedia(GlobalResources.MediaDesktopPlayer);
            }


            public void PauseMedia(MediaDesktopPlayer player)
            {
                if (player.MediaPlayer == this.MediaPlayer)
                    player.Pause();
            }
            public void StopMedia(MediaDesktopPlayer player)
            {
                if(player.MediaPlayer == this.MediaPlayer)
                    player.Stop();  
            }

            /// <summary>
            /// Reload this runtime object with a new media item.
            /// </summary>
            /// <param name="mediaItem"></param>
            public void ReLoad(MediaItem mediaItem)
            {
                EventLogOff();
                Initialize(mediaItem);
            }
            #endregion

            #region Public Properties
            public long MediaLength
            {
                get { return Media.Duration; }
            }

            public float MediaFps
            {
                get { return MediaPlayer.Fps; }
            }

            public Size MediaResolution
            {
                get
                {
                    uint width=0, height = 0;
                    MediaPlayer.Size(0, ref width, ref height);
                    return new Size((int)width,(int)height);
                }
            }

            public float MediaKbps
            {
                get { return MediaPlayer.Media.Statistics.DemuxBitrate; }
            }

            public long MediaSize
            {
                get { return new System.IO.FileInfo(MediaItem.MediaPath).Length; }
            }

            public float MediaKbpsAverage
            {
                get 
                {
                    long time_s = MediaLength / 1000;
                    float kbps = MediaSize / time_s / 125;
                    return kbps;
                }
            }

            public float MediaPlayedProgress
            {
                get
                {
                    if (RelatedMediaDesktopPlayer != null)
                        return RelatedMediaDesktopPlayer.Position;
                    return 0f;
                }
                set
                {
                    if (RelatedMediaDesktopPlayer.Position != value)
                    {
                        RelatedMediaDesktopPlayer.Position = value;
                        OnPropertyChanged(nameof(MediaPlayedProgress));
                    }
                }
            }

            public bool IsMediaDesktopPlayerChangeRequsetCompleted
            {
                get { return isMediaDesktopPlayerChangeRequestCompleted; }
                set
                {
                    if (isMediaDesktopPlayerChangeRequestCompleted != value)
                    {
                        isMediaDesktopPlayerChangeRequestCompleted = value;
                        OnPropertyChanged(nameof(IsMediaDesktopPlayerChangeRequsetCompleted));
                    }
                }
            }

            public long MediaCurrentTime
            {
                get { return (long)(MediaPlayedProgress * MediaLength); }
                set
                {
                    float rate = (float)value / (float)MediaLength;
                    if (MediaPlayer.Position != rate)
                    {
                        MediaPlayer.Position = rate;
                        OnPropertyChanged(nameof(MediaCurrentTime));
                    }
                }
            }

            public bool IsMediaPlaying
            {
                get { return MediaPlayer.IsPlaying; }
            }

            public MediaStrechMode StrechMode
            {
                get { return strechMode; }
                set
                {
                    ApplyStrechMode(value);
                    if (strechMode != value)
                    {
                        strechMode = value;
                        OnPropertyChanged(nameof(StrechMode));
                    }
                }
            }
            #endregion
            #region Notify Event&Method

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                //libVLC MediaPlayer is running in another thread, we have to call dispatcherQueue to do the task.
                Views.Windows.ClientWindow.Instance.DispatcherQueue.TryEnqueue(() => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); });
            }
            #endregion
        }

        public enum ToggleMediaStatusAction
        {
            Pause = 0,
            Play = 1,
            Stop = 2
        }

        public enum MediaStrechMode
        {
            Uniform = 0,
            UniformToFill = 1,
            Strech = 2
        }


        public struct ToggleMediaStatusData
        {
           public MediaDesktopPlayer Player { get; set; }
           public ToggleMediaStatusAction Action { get; set; }
            public ToggleMediaStatusData(MediaDesktopPlayer player,ToggleMediaStatusAction action)
            {
                Player = player;
                Action = action;
            }
        }
    }
}
