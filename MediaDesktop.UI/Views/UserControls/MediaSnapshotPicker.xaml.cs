using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using MediaDesktop.UI.Helpers;
using MediaDesktop.UI.Models;
using MediaDesktop.UI.Services;
using MediaDesktop.UI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.UserControls
{

    public sealed partial class MediaSnapshotPicker : UserControl, INotifyPropertyChanged,IDisposable
    {
        private bool isSliding;
        private MediaPlayer mediaPlayer;
        private Media media;
        private string[] swapChainParam;
        private bool disposedValue;
        private LibVLCOutputHelper libVLCOutputHelper;
        private float precision = 0.5f;
        private bool renderForeground;

        #region Dependency Property Declaration
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(MediaSnapshotPicker), null);
        public static readonly DependencyProperty LibVLCProperty = DependencyProperty.Register("LibVLC", typeof(LibVLC), typeof(MediaSnapshotPicker), null);
        public static readonly DependencyProperty GeneratedBitmapProperty = DependencyProperty.Register("LibVLC",typeof(BitmapImage),typeof(MediaSnapshotPicker), null);
        public static readonly DependencyProperty ExtractedImagePathProperty = DependencyProperty.Register("ExtractedImagePath", typeof(string), typeof(MediaSnapshotPicker), null);

        public event EventHandler<string> FrameExtracted;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); InitializeBitmapSource(); }
        }

        public LibVLC LibVLC
        {
            get { return (LibVLC)GetValue(LibVLCProperty); }
            private set { SetValue(LibVLCProperty, value); }
        }
        public BitmapImage GeneratedBitmap
        {
            get { return (BitmapImage)GetValue(GeneratedBitmapProperty); }
            private set { SetValue(GeneratedBitmapProperty, value); }
        }

        public string ExtractedImagePath
        {
            get { return (string)GetValue(ExtractedImagePathProperty); }
            private set { SetValue(ExtractedImagePathProperty, value); }
        }

        private void InitializeMediaPlayer()
        {
            //if (swapChainParam is null)
            //    return;

            //if (LibVLC is null)
            //    LibVLC = new LibVLC(enableDebugLogs: true,swapChainParam);
            //DisposeMedia();

            //media = new Media(LibVLC, new Uri(FilePath));

            //media.Parse().Wait();
            //mediaPlayer = new MediaPlayer(media);
            //mediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
            //mediaPlayer.Media.StateChanged += Media_StateChanged;
            //mediaPlayer.EndReached += MediaPlayer_EndReached;

            //videoView.MediaPlayer = mediaPlayer;
            //mediaPlayer.Mute = true;
            //mediaPlayer.Play();
        }

        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {
            DispatcherQueue.TryEnqueue(() => {
                mediaPlayer.Media = mediaPlayer.Media;
                mediaPlayer.Play();
            });
        }

        private void Media_StateChanged(object sender, MediaStateChangedEventArgs e)
        {
            switch(e.State)
            {
                case VLCState.Playing:
                    DispatcherQueue.TryEnqueue(() => { togglePlayStatusButton.Content = '\xE769'; }); break;
                default:
                    DispatcherQueue.TryEnqueue(() => { togglePlayStatusButton.Content = '\xE768'; });break;
            }
        }

        private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            slider.DispatcherQueue.TryEnqueue(() => { slider.Value = e.Position; });
        }

        private void MediaPlayerEventStartup()
        {
            mediaPlayer = new MediaPlayer(media);
            mediaPlayer.Muted += MediaPlayer_Muted;
            mediaPlayer.Unmuted += MediaPlayer_Unmuted;
            mediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
            mediaPlayer.Media.StateChanged += Media_StateChanged;
            mediaPlayer.EndReached += MediaPlayer_EndReached;
        }

        private void MediaPlayer_Unmuted(object sender, EventArgs e)
        {
            muteButton.Content = "\xE767";
        }

        private void MediaPlayer_Muted(object sender, EventArgs e)
        {
            muteButton.Content = "\xE74F";
        }

        public void InitializeBitmapSource()
        {
            DisposeMedia();
            if (LibVLC is null)
                LibVLC = GlobalResources.LibVLC;
            media = new Media(LibVLC, new Uri(FilePath));
            media.Parse(MediaParseOptions.ParseLocal).Wait();
            mediaPlayer = new MediaPlayer(media);
            MediaPlayerEventStartup();
            libVLCOutputHelper = new LibVLCOutputHelper(mediaPlayer, (uint)(media.Tracks[0].Data.Video.Width * precision), (uint)(media.Tracks[0].Data.Video.Height * precision));
            libVLCOutputHelper.BitmapRenderCompleted += (_, __) =>
            {
                if (renderForeground)
                {
                    libVLCOutputHelper.SetOutputBitmapSource(foreBitmapSource);
                    DispatcherQueue.TryEnqueue(() => { foreBitmapImage.Opacity = 1; });
                }
                else
                {
                    libVLCOutputHelper.SetOutputBitmapSource(backBitmapSource);
                    DispatcherQueue.TryEnqueue(() => { foreBitmapImage.Opacity = 0; });
                }
                renderForeground = !renderForeground;
            };
            mediaPlayer.Mute = true;
            mediaPlayer.Media = media;
            mediaPlayer.Play();
        }

        //public void UpdateGeneratedImages()
        //{
        //    if (LibVLC is null)
        //        LibVLC = GlobalResources.LibVLC;

        //    if (!File.Exists(FilePath) || LibVLC is null)
        //        return;

        //    generatedImages.Clear();
        //    string filePath = FilePath;
        //    LibVLC libvlc = LibVLC;
        //    TimeSpan frameTimeSpan = FrameTimeSpan;
        //    long timeSpan = (long)frameTimeSpan.TotalMilliseconds;
        //    Media originalMedia = new Media(LibVLC, new Uri(filePath));
        //    originalMedia.Parse(MediaParseOptions.ParseLocal).Wait();
        //    Task.Run(() =>
        //    {
        //        for (long i = 0; i < originalMedia.Duration; i += timeSpan)
        //        {
        //            mediaPlayer = new MediaPlayer(originalMedia.Duplicate());
        //            LibVLCOutputHelper libVLCOutputHelper = new LibVLCOutputHelper(mediaPlayer, 300, 169);
        //            mediaPlayer.Playing += (_, __) => semaphore2.Release();
        //            mediaPlayer.Play();

        //            generateFrameForNextFrame = true;
        //            libVLCOutputHelper.BitmapRenderCompleted += LibVLCOutputHelper_BitmapRenderCompleted;
        //            semaphore2.Wait();
        //            mediaPlayer.SeekTo(TimeSpan.FromMilliseconds(i));
        //            mediaPlayer.SetPause(true);
        //            semaphore.Wait();
        //            libVLCOutputHelper.BitmapRenderCompleted -= LibVLCOutputHelper_BitmapRenderCompleted;
        //            mediaPlayer.Stop();
        //            mediaPlayer.Dispose();
        //        }
        //    });
        //}

        public MediaSnapshotPicker()
        {
            this.InitializeComponent();
            slider.AddHandler(PointerPressedEvent, new PointerEventHandler(Slider_PointerPressed), true);
            slider.AddHandler(PointerReleasedEvent, new PointerEventHandler(Slider_PointerReleased), true);
        }

        private void VideoView_Initialized(object sender, LibVLCSharp.Platforms.Windows.InitializedEventArgs e)
        {
            swapChainParam = e.SwapChainOptions;
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!isSliding || mediaPlayer is null)
                return;
            mediaPlayer.Position = (float)slider.Value;
        }
        private void Slider_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isSliding = true;
            mediaPlayer?.SetPause(true);
            Slider_ValueChanged(slider, null);
        }
        private void Slider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isSliding = false;
            mediaPlayer?.SetPause(false);
        }
        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer is null) return;


            string path = GetExpectedExtractFilePath();
            mediaPlayer.TakeSnapshot(0, path, 1280, 720);
            FrameExtracted?.Invoke(this, path);
            ExtractedImagePath = path;
        }

        /// <summary>
        /// Gets expected extract file full path, based on current app settings.
        /// </summary>
        /// <returns></returns>
        public string GetExpectedExtractFilePath()
        {
            ExtractedFramePosition position = GlobalResources.ViewModelCollection.SettingsItemViewModel.ExtractedFramePosition;
            ExtractedFrameNamePolicy namePolicy = GlobalResources.ViewModelCollection.SettingsItemViewModel.ExtractedFrameNamePolicy;
            string basePath, fileName;
            switch (position)
            {
                case ExtractedFramePosition.SourceFolder:
                    basePath = Path.GetDirectoryName(FilePath); break;
                case ExtractedFramePosition.LocalCacheFolder:
                    basePath = ApplicationData.Current.LocalCacheFolder.Path; break;
                case ExtractedFramePosition.UserPictureFolder:
                    basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); break;
                default: basePath = Path.GetDirectoryName(FilePath); break;
            }

            switch (namePolicy)
            {
                case ExtractedFrameNamePolicy.AppendExtension:
                    fileName = Path.GetFileName(FilePath) + ".png"; break;
                case ExtractedFrameNamePolicy.OverrideExtension:
                    fileName = Path.GetFileNameWithoutExtension(FilePath) + ".png"; break;
                case ExtractedFrameNamePolicy.RandomHex8:
                    fileName = new Random().Next().ToString("X16") + ".png"; break;
                case ExtractedFrameNamePolicy.RandomHex16:
                    fileName = new Random().NextInt64().ToString("X16") + ".png"; break;
                default: fileName = Path.GetFileNameWithoutExtension(FilePath) + ".png"; break;
            }

            string path = Path.Combine(basePath, fileName);
            return path;
        }

        private void DisposeMedia()
        {
            mediaPlayer?.Stop();
            media?.Dispose();
            mediaPlayer?.Dispose();
            libVLCOutputHelper?.Dispose();
            libVLCOutputHelper = null;

            mediaPlayer = null;
            media = null;
        }

        private void TogglePlayStatusButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer?.Pause();
        }

        ~MediaSnapshotPicker()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    DisposeMedia();
                    //LibVLC?.Dispose();
                    //LibVLC = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~MediaSnapshotPicker()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer?.ToggleMute();
        }
    }
}
