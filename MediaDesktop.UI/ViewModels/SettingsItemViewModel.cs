using MediaDesktop.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using IniParser;
using IniParser.Model;
using MediaDesktop.UI.Services;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using MediaDesktop.UI.Views.Windows;
using Windows.Storage.Pickers;
using System.Threading;
using MediaDesktop.UI.Helpers.Extensions;
using LibVLCSharp.Shared;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using MediaDesktop.UI.Views.Pages;
using MediaDesktop.UI.Helpers;

namespace MediaDesktop.UI.ViewModels
{
    public class SettingsItemViewModel : INotifyPropertyChanged
    {
        private SettingsItem settingsItem;
        private readonly string defaultKey = "Value";
        private readonly string defaultItemRecordPath = ApplicationData.Current.LocalFolder.Path + @"\items.ini";
        private readonly string defaultExceptionLogPath = ApplicationData.Current.LocalFolder.Path + @"\exceptionlog.ini";
        private readonly string defaultMediaPlayingListDir = ApplicationData.Current.LocalFolder.Path + @"\PlayingList";
        private readonly string basePath = ApplicationData.Current.LocalFolder.Path + @"\base.ini";
        private readonly string baseDirectory = ApplicationData.Current.LocalFolder.Path;
        private FileIniDataParser FileIniDataParser { get { return GlobalResources.FileIniDataParser; } }

        public string BasePath
        {
            get { return basePath; }
        }

        public string BaseDirectory
        {
            get { return baseDirectory; }
        }

        #region MVVM Settings Data
        public float LibraryItemScale
        {
            get { return settingsItem.LibraryItemScale; }
            set
            {
                if (settingsItem.LibraryItemScale != value)
                {
                    if(value >= 0.4 && value <=1.8)
                    settingsItem.LibraryItemScale = value;
                    OnPropertyChanged(nameof(LibraryItemScale));
                }
            }
        }
        public string MediaItemRecordINIPath
        {
            get { return settingsItem.MediaItemRecordINIPath; }
            set
            {
                if (settingsItem.MediaItemRecordINIPath != value)
                {
                    settingsItem.MediaItemRecordINIPath = value;
                    OnPropertyChanged(nameof(MediaItemRecordINIPath));
                }
            }
        }

        public string MediaPlayingListINIDir
        {
            get { return settingsItem.MediaPlayingListINIDir; }
            set
            {
                if (settingsItem.MediaPlayingListINIDir != value)
                {
                    settingsItem.MediaPlayingListINIDir = value;
                    OnPropertyChanged(nameof(MediaPlayingListINIDir));
                }
            }
        }

        public string ExceptionLogPath
        {
            get { return settingsItem.ExceptionLogPath; }
            set
            {
                if (settingsItem.ExceptionLogPath != value)
                {
                    settingsItem.ExceptionLogPath = value;
                    OnPropertyChanged(nameof(ExceptionLogPath));
                }
            }
        }
        public int Volume
        {
            get { return settingsItem.Volume; }
            set { if(settingsItem.Volume!=value)
                {
                    settingsItem.Volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        public float SpeedRatio
        {
            get { return settingsItem.SpeedRatio; }
            set
            {
                if (settingsItem.SpeedRatio != value)
                {
                    settingsItem.SpeedRatio = value;
                    OnPropertyChanged(nameof(SpeedRatio));
                }
            }
        }

        public int LastLibraryPagePivotIndex
        {
            get { return settingsItem.LastLibraryPagePivotIndex; }
            set
            {
                if (settingsItem.LastLibraryPagePivotIndex != value)
                {
                    settingsItem.LastLibraryPagePivotIndex = value;
                    OnPropertyChanged(nameof(LastLibraryPagePivotIndex));
                }
            }
        }

        public PlayBackMode PlayBackMode
        {
            get { return settingsItem.PlayBackMode; }
            set
            {
                if (settingsItem.PlayBackMode != value)
                {
                    settingsItem.PlayBackMode = value;
                    OnPropertyChanged(nameof(PlayBackMode));
                }
            }
        }

        public bool EnableHardwareDecoding
        {
            get { return settingsItem.EnableHardwareDecoding; }
            set
            {
                if(settingsItem.EnableHardwareDecoding != value)
                {
                    settingsItem.EnableHardwareDecoding = value;
                    OnPropertyChanged(nameof(EnableHardwareDecoding));
                }
            }
        }

        public float TintOpacity
        {
            get { return settingsItem.TintOpacity; }
            set { settingsItem.TintOpacity = value; OnPropertyChanged(nameof(TintOpacity)); }
        }

        public float LuminosityOpacity
        {
            get { return settingsItem.LuminosityOpacity; }
            set { settingsItem.LuminosityOpacity = value;OnPropertyChanged(nameof(LuminosityOpacity)); }
        }

        public Windows.UI.Color TintColor
        {
            get { return settingsItem.TintColor; }
            set { 
                settingsItem.TintColor = value;
                OnPropertyChanged(nameof(TintColor));
            }   
        }

        public WindowBackdropStyle AcrylicMicaStyle
        {
            get { return settingsItem.AcrylicMicaStyle; }
            set
            {
                settingsItem.AcrylicMicaStyle = value;
                OnPropertyChanged(nameof(AcrylicMicaStyle));
            }
        }

        public int FileCaching
        {
            get { return settingsItem.FileCaching; }
            set
            {
                settingsItem.FileCaching = value;
                OnPropertyChanged(nameof(FileCaching));
            }
        }

        public bool IsNavigationViewMaterialEnabled
        {
            get { return settingsItem.IsNavigationViewMaterialEnabled; }
            set
            {
                if (settingsItem.IsNavigationViewMaterialEnabled != value)
                {
                    settingsItem.IsNavigationViewMaterialEnabled = value;
                    OnPropertyChanged(nameof(IsNavigationViewMaterialEnabled));
                }
            }
        }

        public bool IsPlayBackControlMaterialEnabled
        {
            get { return settingsItem.IsPlayBackControlMaterialEnabled; }
            set
            {
                if (settingsItem.IsPlayBackControlMaterialEnabled != value)
                {
                    settingsItem.IsPlayBackControlMaterialEnabled = value;
                    OnPropertyChanged(nameof(IsPlayBackControlMaterialEnabled));
                }
            }
        }
        public bool IsPageMaterialEnabled
        {
            get { return settingsItem.IsPageMaterialEnabled; }
            set
            {
                if (settingsItem.IsPageMaterialEnabled != value)
                {
                    settingsItem.IsPageMaterialEnabled = value;
                    OnPropertyChanged(nameof(IsPageMaterialEnabled));
                }
            }
        }

        public bool IsTitleBarMaterialEnabled
        {
            get { return settingsItem.IsTitleBarMaterialEnabled; }
            set
            {
                if (settingsItem.IsTitleBarMaterialEnabled != value)
                {
                    settingsItem.IsTitleBarMaterialEnabled = value;
                    OnPropertyChanged(nameof(IsTitleBarMaterialEnabled));
                }
            }
        }

        #endregion

        public void Save()
        {
            IniData iniData = EncodeIniData();
            FileIniDataParser.WriteFile(basePath, iniData);
        }
        public void SwitchPlayBackMode()
        {
            if (PlayBackMode == PlayBackMode.Shuffle)
                PlayBackMode = PlayBackMode.RepeatOne;
            else PlayBackMode = PlayBackMode.Shuffle;
        }

        public async Task BrowseFileInExplorer(string path,XamlRoot xamlRoot)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "要在Windows资源管理器中浏览配置文件吗？",
                Content = "这将在新应用中打开。",
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = "是",
                SecondaryButtonText = "否",
                XamlRoot = xamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Secondary)
                return;

            if (File.Exists(path))
            {
                WinUI3Helper.SelectFileInExplorer(path);
            }
            else
            {
                await new ContentDialog()
                {
                    Title = "无法定位配置文件",
                    PrimaryButtonText = "关闭",
                    DefaultButton = ContentDialogButton.Primary,
                    Content = "配置文件不存在。\n\n如果这是您第一次启动本程序，则属正常情况。\n配置文件将会在您退出程序时尝试生成。",
                    XamlRoot = xamlRoot
                }.ShowAsync();
            }
        }

        public async void SetMediaRecordPath()
        {
            FileSavePicker picker = new FileSavePicker();
            WinUI3Helper.PickerInitializeWindow(Views.Windows.ClientWindow.Instance, picker);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = "MediaDesktop.UI.Library";
            picker.FileTypeChoices.Add("配置文件", new List<string>() { ".ini" });
            StorageFile file = await picker.PickSaveFileAsync();
            if(file is not null)
            {
                MediaItemRecordINIPath = file.Path;
            }
        }

        public async void SetExceptionLogPath()
        {
            FileSavePicker picker = new FileSavePicker();
            WinUI3Helper.PickerInitializeWindow(Views.Windows.ClientWindow.Instance, picker);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = "MediaDesktop.UI.ExceptionLog";
            picker.FileTypeChoices.Add("日志文件", new List<string>() { ".ini" });
            StorageFile file = await picker.PickSaveFileAsync();
            if (file is not null)
            {
                ExceptionLogPath = file.Path;
            }
        }

        public async void SetMediaPlayingListDir()
        {
            FolderPicker picker = new FolderPicker();
            WinUI3Helper.PickerInitializeWindow(Views.Windows.ClientWindow.Instance, picker);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*");
            StorageFolder folder = await picker.PickSingleFolderAsync();
            if (folder is not null)
            {
                MediaPlayingListINIDir = folder.Path;
            }
        }
       

        #region Delegate Command
        public DelegateCommand SwitchPlayBackModeCommand { get; private set; }
        public DelegateCommand BrowseFileInExplorerCommand { get; private set; }
        public DelegateCommand SetMediaRecordPathCommand { get; private set; }
        public DelegateCommand SetExceptionLogPathCommand { get; private set; }
        public DelegateCommand SetMediaPlayingListDirCommand { get; private set; }
        public DelegateCommand SetSpeedRatioCommand { get; private set; }
        #endregion
        private void DelegateCommandStartup()
        {
            SwitchPlayBackModeCommand = new DelegateCommand((obj) => { SwitchPlayBackMode(); });
            BrowseFileInExplorerCommand = new DelegateCommand(async(obj) => {await BrowseFileInExplorer(obj as string,ClientHostPage.Instance.XamlRoot); });
            SetMediaRecordPathCommand = new DelegateCommand((obj) => { SetMediaRecordPath(); });
            SetExceptionLogPathCommand = new DelegateCommand((obj) => { SetExceptionLogPath(); });
            SetMediaPlayingListDirCommand = new DelegateCommand((obj) => { SetMediaPlayingListDir(); });
            SetSpeedRatioCommand = new DelegateCommand(obj => SpeedRatio = Convert.ToSingle(obj));
        }

        /// <summary>
        /// Initialize <see cref="SettingsItemViewModel"/> by reading ini config file. When adding new property reading operation in this method,
        /// remember to add corresponding encoding operation in <seealso cref="EncodeIniData"/>
        /// </summary>
        public void Initialize()
        {
            if (!File.Exists(basePath))
            {
                File.Create(basePath).Close();
            }
            IniData iniData = FileIniDataParser.ReadFile(basePath);
            MediaItemRecordINIPath = iniData.GetStringValueOrDefault(nameof(MediaItemRecordINIPath), defaultKey, defaultItemRecordPath);
            MediaPlayingListINIDir = iniData.GetStringValueOrDefault(nameof(MediaPlayingListINIDir), defaultKey, defaultMediaPlayingListDir);
            ExceptionLogPath = iniData.GetStringValueOrDefault(nameof(ExceptionLogPath), defaultKey, defaultExceptionLogPath);
            Volume = iniData.GetValueTypeValueOrDefault(nameof(Volume), defaultKey, 100);
            SpeedRatio = iniData.GetValueTypeValueOrDefault(nameof(SpeedRatio), defaultKey, 1F);
            PlayBackMode = (PlayBackMode)Enum.Parse(typeof(PlayBackMode), iniData.GetStringValueOrDefault(nameof(PlayBackMode), defaultKey, "RepeatOne"));
            LastLibraryPagePivotIndex = iniData.GetValueTypeValueOrDefault(nameof(LastLibraryPagePivotIndex), defaultKey, 0);
            EnableHardwareDecoding = iniData.GetValueTypeValueOrDefault(nameof(EnableHardwareDecoding), defaultKey, true);
            //TintColor = iniData.GetWindowsUIColorOrDefault(nameof(TintColor), defaultKey, (Windows.UI.Color)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"]); //Black
            TintColor = ((SolidColorBrush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"]).Color;
            TintOpacity = iniData.GetValueTypeValueOrDefault(nameof(TintOpacity), defaultKey, 0.8f);
            LuminosityOpacity = iniData.GetValueTypeValueOrDefault(nameof(LuminosityOpacity), defaultKey, 0.16f);
            AcrylicMicaStyle = iniData.GetValueTypeValueOrDefault(nameof(AcrylicMicaStyle), defaultKey, WindowBackdropStyle.None);
            FileCaching = iniData.GetValueTypeValueOrDefault(nameof(FileCaching), defaultKey, 500);
            IsNavigationViewMaterialEnabled = iniData.GetValueTypeValueOrDefault(nameof(IsNavigationViewMaterialEnabled), defaultKey, true);
            IsPageMaterialEnabled = iniData.GetValueTypeValueOrDefault(nameof(IsPageMaterialEnabled), defaultKey, true);
            IsPlayBackControlMaterialEnabled = iniData.GetValueTypeValueOrDefault(nameof(IsPlayBackControlMaterialEnabled), defaultKey, true);
            IsTitleBarMaterialEnabled = iniData.GetValueTypeValueOrDefault(nameof(IsTitleBarMaterialEnabled), defaultKey, true);
            LibraryItemScale = iniData.GetValueTypeValueOrDefault(nameof(LibraryItemScale), defaultKey,1.0f);
        }

        public SettingsItemViewModel()
        {
            settingsItem = new SettingsItem();
            EventStartup();
            DelegateCommandStartup();
            Initialize();
        }

        private IniData EncodeIniData()
        {
            IniData data = new IniData();
            data.Sections.AddSection(nameof(MediaItemRecordINIPath));
            data.Sections.AddSection(nameof(MediaPlayingListINIDir));
            data.Sections.AddSection(nameof(ExceptionLogPath));
            data.Sections.AddSection(nameof(Volume));
            data.Sections.AddSection(nameof(PlayBackMode));
            data.Sections.AddSection(nameof(SpeedRatio));
            data.Sections.AddSection(nameof(LastLibraryPagePivotIndex));
            data.Sections.AddSection(nameof(EnableHardwareDecoding));
            data.Sections.AddSection(nameof(TintOpacity));
            data.Sections.AddSection(nameof(LuminosityOpacity));
            data.Sections.AddSection(nameof(TintColor));
            data.Sections.AddSection(nameof(AcrylicMicaStyle));
            data.Sections.AddSection(nameof(FileCaching));
            data.Sections.AddSection(nameof(IsNavigationViewMaterialEnabled));
            data.Sections.AddSection(nameof(IsPageMaterialEnabled));
            data.Sections.AddSection(nameof(IsPlayBackControlMaterialEnabled));
            data.Sections.AddSection(nameof(IsTitleBarMaterialEnabled));
            data.Sections.AddSection(nameof(LibraryItemScale));

            data.Sections[nameof(MediaItemRecordINIPath)].AddKey(defaultKey, MediaItemRecordINIPath);
            data.Sections[nameof(MediaPlayingListINIDir)].AddKey(defaultKey, MediaPlayingListINIDir);
            data.Sections[nameof(ExceptionLogPath)].AddKey(defaultKey, ExceptionLogPath);
            data.Sections[nameof(Volume)].AddKey(defaultKey, Volume.ToString());
            data.Sections[nameof(PlayBackMode)].AddKey(defaultKey, PlayBackMode.ToString());
            data.Sections[nameof(SpeedRatio)].AddKey(defaultKey, SpeedRatio.ToString());
            data.Sections[nameof(LastLibraryPagePivotIndex)].AddKey(defaultKey, LastLibraryPagePivotIndex.ToString());
            data.Sections[nameof(EnableHardwareDecoding)].AddKey(defaultKey, EnableHardwareDecoding.ToString());
            data.Sections[nameof(TintOpacity)].AddKey(defaultKey, TintOpacity.ToString());
            data.Sections[nameof(LuminosityOpacity)].AddKey(defaultKey, LuminosityOpacity.ToString());
            data.Sections[nameof(TintColor)].AddKey(defaultKey, TintColor.ToString());
            data.Sections[nameof(AcrylicMicaStyle)].AddKey(defaultKey, AcrylicMicaStyle.ToString());
            data.Sections[nameof(IsNavigationViewMaterialEnabled)].AddKey(defaultKey, IsNavigationViewMaterialEnabled.ToString());
            data.Sections[nameof(FileCaching)].AddKey(defaultKey, FileCaching.ToString());
            data.Sections[nameof(IsPageMaterialEnabled)].AddKey(defaultKey, IsPageMaterialEnabled.ToString());
            data.Sections[nameof(IsPlayBackControlMaterialEnabled)].AddKey(defaultKey, IsPlayBackControlMaterialEnabled.ToString());
            data.Sections[nameof(IsTitleBarMaterialEnabled)].AddKey(defaultKey, IsTitleBarMaterialEnabled.ToString());
            data.Sections[nameof(LibraryItemScale)].AddKey(defaultKey, LibraryItemScale.ToString());
            return data;
        }

        private void EventStartup()
        {
            PropertyChanged += This_PropertyChanged;
            GlobalResources.MediaDesktopPlayer.MediaPlayerPlaying += MediaDesktopPlayer_MediaPlayerPlaying;

        }

        /// <summary>
        /// When media is changing, set its parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MediaDesktopPlayer_MediaPlayerPlaying(object sender, LibVLCSharp.Shared.MediaPlayer args)
        {
            Task.Run(() =>
            {
                args.Mute = true;
                Thread.Sleep(150);
                args.Volume = Volume;
                args.Mute = false;
                args.SetRate(SpeedRatio);
            });
        }

        private void This_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(MediaItemRecordINIPath):

                    break;

                    //Volume settings is being changed.
                case nameof(Volume):
                    if (GlobalResources.MediaDesktopPlayer.MediaPlayer != null)
                    {
                        GlobalResources.MediaDesktopPlayer.MediaPlayer.Volume = Volume;
                    }
                    break;

                case nameof(SpeedRatio):
                    if (GlobalResources.MediaDesktopPlayer.MediaPlayer != null)
                    {
                        GlobalResources.MediaDesktopPlayer.MediaPlayer.SetRate(SpeedRatio);
                    }
                    break;
                case nameof(FileCaching):
                    var viewModels = GlobalResources.ViewModelCollection?.ViewModelItems ?? null;
                    if (viewModels != null)
                    {
                        foreach (var viewModel in viewModels.Where(v => v.MediaItemViewModel.RuntimeDataSet.IsMediaPlaying is false))
                        {
                            viewModel.MediaItemViewModel.UpdateMediaFileCaching(FileCaching);
                        }
                    }
                    break;
                case nameof(TintOpacity):
                case nameof(AcrylicMicaStyle):
                case nameof(TintColor):
                case nameof(LuminosityOpacity):
                    var color = ((SolidColorBrush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"]).Color;
                    var clientWindow = ClientWindow.Instance;
                    clientWindow.SetBackdropStyle(AcrylicMicaStyle, TintOpacity, LuminosityOpacity, color);
                    WindowManager.WinApis.UnEncapsulated.WinApi.SetForegroundWindow(GlobalResources.MediaDesktopBase.ProgramManagerWindowHandle);
                    clientWindow.Activate();
                    break;
                case nameof(IsTitleBarMaterialEnabled): //titlebar element border1 and its layer mask are defined in ClientWindow, where binding expressions are disabled. Here we manually handle its acrylic style.
                    ClientWindow.TitleBarMaskGrid.Visibility = IsTitleBarMaterialEnabled ? Visibility.Collapsed : Visibility.Visible;
                    break;
            }

        }
        #region Notify Events&Methods
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        }
        #endregion
    }
}
