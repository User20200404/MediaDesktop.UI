using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using MediaDesktop.UI.Helpers;
using MediaDesktop.UI.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.UserControls
{
    public sealed partial class MediaSnapshotPicker : UserControl, INotifyPropertyChanged
    {
        #region Dependency Property Declaration
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(MediaSnapshotPicker), null);
        public static readonly DependencyProperty SourceStreamProperty = DependencyProperty.Register("SourceStream", typeof(Stream), typeof(MediaSnapshotPicker), null);
        public static readonly DependencyProperty FrameTimeSpanProperty = DependencyProperty.Register("FrameTimeSpan", typeof(TimeSpan), typeof(MediaSnapshotPicker), new PropertyMetadata(TimeSpan.FromSeconds(30)));
        public static readonly DependencyProperty LibVLCProperty = DependencyProperty.Register("LibVLC", typeof(LibVLC), typeof(MediaSnapshotPicker), null);



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private bool renderForeground;
        private MediaPlayer mediaPlayer;
        private LibVLCOutputHelper libVLCOutputHelper;
        float percision = 0.2f;
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value);InitializeBitmapSource(); }
        }

        public Stream SourceStream
        {
            get { return (Stream)GetValue(SourceStreamProperty); }
            set { SetValue(SourceStreamProperty, value); }
        }
        public TimeSpan FrameTimeSpan
        {
            get { return (TimeSpan)GetValue(FrameTimeSpanProperty); }
            set { SetValue(FrameTimeSpanProperty, value); }
        }

        public LibVLC LibVLC
        {
            get { return (LibVLC)GetValue(LibVLCProperty); }
            set { SetValue(LibVLCProperty, value); }
        }

        public void InitializeBitmapSource()
        {
            if (libVLCOutputHelper != null)
            {
                libVLCOutputHelper.Dispose();
            }
            if (LibVLC is null)
                LibVLC = GlobalResources.LibVLC;
            mediaPlayer = new MediaPlayer(LibVLC);
            Media media = new Media(LibVLC, new Uri(FilePath));
            media.Parse(MediaParseOptions.ParseLocal).Wait();
            libVLCOutputHelper = new LibVLCOutputHelper(mediaPlayer, (uint)(media.Tracks[0].Data.Video.Width * percision), (uint)(media.Tracks[0].Data.Video.Height * percision));
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
        }
    }
}
