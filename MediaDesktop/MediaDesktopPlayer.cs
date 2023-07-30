using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using WindowManager;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MediaDesktop
{
    public delegate void MediaPlayerChangeEventHandler(object sender, MediaPlayerChangeEventArgs args);
    public delegate void MediaPlayerPlayingEventHandler(object sender, MediaPlayer args);
    public class MediaDesktopPlayer
    {
        private bool canChangeMediaPlayerStatus = true;
        private VideoView mediaPresenter;
        private VideoView mediaPresenter_alpha;
        private VideoView currentMediaPresenter;
        private LibVLC libVLC;
        private MediaPlayer mediaPlayer;

        private VideoView BackMediaPresenter
        {
            get
            {
                if (currentMediaPresenter == mediaPresenter)
                {
                    return mediaPresenter_alpha;
                }
                else
                {
                    return mediaPresenter;
                }
            }
        }
        public event EventHandler ScreenSolutionChanged;
        /// <summary>
        /// MediaPlayer is about to change.
        /// </summary>
        public event MediaPlayerChangeEventHandler MediaPlayerChanging;

        /// <summary>
        /// MediaPlayer has changed.
        /// </summary>
        public event MediaPlayerChangeEventHandler MediaPlayerChanged;

        /// <summary>
        /// MediaPlayer change request has completed. This means you can change MediaPlayer now.
        /// </summary>
        public event MediaPlayerChangeEventHandler MediaPlayerChangeRequestCompleted;
        public event MediaPlayerPlayingEventHandler MediaPlayerPlaying;
        public event EventHandler MediaPlayerEndReached;

        /// <summary>
        /// Gets the instance of <see cref="LibVLCSharp.Shared.LibVLC"/> for current player.
        /// </summary>
        public LibVLC LibVLC
        {
            set { libVLC = value; }
            get { return libVLC; }
        }

        /// <summary>
        /// Gets a value that indicates whether MediaPlayer status can be changed(Includes change MediaPlayer, Position and so on).
        /// </summary>
        public bool CanChangeMediaPlayerStatus => canChangeMediaPlayerStatus;

        /// <summary>
        /// Get the instance of <see cref="VideoView"/> for current player (a system-managed window, you can control it using WinForms methods or Windows APIs).
        /// </summary>
        public VideoView MediaPresenter
        {
            get { return mediaPresenter; }
        }

        public float Position
        {
            get { return mediaPlayer.Position; }
            set
            {
                if (canChangeMediaPlayerStatus)
                    mediaPlayer.Position = value;
            }
        }

        /// <summary>
        /// Get the instance of <see cref="LibVLCSharp.Shared.MediaPlayer"/> to control the media-play better.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get { return mediaPlayer; }
            set
            {
                if (!canChangeMediaPlayerStatus)
                {
                    Debug.WriteLine($"[{DateTime.Now}] MediaPlayer switch request rejected - Last request has not completed yet.");
                    return; 
                }

                if (mediaPlayer != value)
                {
                    Debug.WriteLine($"[{DateTime.Now}] {nameof(canChangeMediaPlayerStatus)} set to true at MediaDesktopPlayer.MediaPlayer.set");
                    canChangeMediaPlayerStatus = false;
                    MediaPlayerChangeEventArgs args = new MediaPlayerChangeEventArgs(BackMediaPresenter.MediaPlayer, value);
                    This_MediaPlayerChanging(this, args);

                    MediaPlayerChanging?.Invoke(this, args);
                    mediaPlayer = value;
                    MediaPlayerChanged?.Invoke(this, args);
                }
            }
        }

        private void EventStartup()
        {
            ReceiverForm.Instance.ScreenSolutionChanged += System_ScreenSolutionChanged;

            //const int WH_CALLWNDPROC = 12;

            //int msgReceiverTid = APIsPackaged.GetThreadIdByWindowHandle(messageReceiver.Handle);
            //hReceiverHook = SystemAPIs.SetWindowsHookExA(WH_CALLWNDPROC, WndProcCallBack, IntPtr.Zero, msgReceiverTid);
            //Install hook to handle screenSolution changed events.
            //the target is current thread so hInstance should be zero.

            //IntPtr hwnd = APIsPackaged.FindProgramManagers()[0];
            //int tid = APIsPackaged.GetThreadIdByWindowHandle(hwnd);
            //IntPtr hDll = SystemAPIs.LoadLibraryA(Application.StartupPath + "Resources\\WndCallBack.dll");
            //Debug.WriteLine( Marshal.GetLastWin32Error());
            //IntPtr hFunc = SystemAPIs.GetProcAddress(hDll, "ResizeWndCallBack");
            //hProgManHook = SystemAPIs.SetWindowsHookExA(WH_CALLWNDPROC, hFunc, hDll, tid);
            //SystemAPIs.FreeLibrary(hDll);
        }

        private void System_ScreenSolutionChanged(object sender, EventArgs e)
        {
            ResizePlayerToScreen();
            ScreenSolutionChanged(sender,e);
        }

        //private unsafe int WndProcCallBack(int nCode, uint wParam, IntPtr lParam)
        //{
        //    const int WM_USER_RESIZE = 1025;
        //    const int WM_SOLUTIONCHANGED = 0x007e;
        //    WIN32tagCWPRETSTRUCT* wndStruct = (WIN32tagCWPRETSTRUCT*)lParam;
        //    if (wndStruct->message == WM_SOLUTIONCHANGED)
        //    {
        //        ResizePlayerToScreen();
        //    }
        //    return 0;
        //}

        /// <summary>
        /// Switches <seealso cref="currentMediaPresenter"/> to another one.
        /// </summary>
        private void SwitchCurrentPresenter()
        {
            if (currentMediaPresenter == mediaPresenter)
                currentMediaPresenter = mediaPresenter_alpha;
            else currentMediaPresenter = mediaPresenter;
        }

        /// <summary>
        /// Deals with the mediaplayer change events. TODO: Register events for new MediaPlayer and unregister events for old MediaPlayer. Notice that some events should be lately unregistered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void This_MediaPlayerChanging(object sender, MediaPlayerChangeEventArgs args)
        {
            //(Obsolete)Without this command, MediaDesktop.UI may mistakenly recognize the CurrentPlayingItem (the first item where IsMediaPlaying == true).
            //args.OldMediaPlayer?.Pause();

            currentMediaPresenter.MediaPlayer = args.NewMediaPlayer;
            SwitchCurrentPresenter();
            Debug.WriteLine($"[{DateTime.Now}] {nameof(MediaPresenter)} switched.");

            if (args.NewMediaPlayer != null)
            {
                args.NewMediaPlayer.EndReached += MediaPlayer_EndReached;
                args.NewMediaPlayer.Playing += NewMediaPlayer_Playing;
                args.NewMediaPlayer.PositionChanged += NewMediaPlayer_PositionChanged;
            }

            if(args.OldMediaPlayer != null)
            {
                args.OldMediaPlayer.EndReached -= MediaPlayer_EndReached; 
                args.OldMediaPlayer.Playing -= NewMediaPlayer_Playing;
                //We unregister OldMediaPlayer.PositionChanged event at NewMediaPlayer_PositionChanged instead of here.
            }

        }


        /// <summary>
        /// To ensure the new media playback started, use this event to switch mediaPresenter properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewMediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            //New media playback succeed. TODO: 1.Unregister old player's PositionChanged event;  2.Show the new presenter to end user;  3.Stop the old media's playback.

            //BackMediaPresenter is where uses the old media player. We unregister its event here.
            BackMediaPresenter.MediaPlayer.PositionChanged -= NewMediaPlayer_PositionChanged;

            canChangeMediaPlayerStatus = true;
            Debug.WriteLine("[" + DateTime.Now.ToString() + $"]{nameof(canChangeMediaPlayerStatus)} set to {canChangeMediaPlayerStatus} at NewMediaPlayer_PositionChanged.");

            MediaPlayerChangeEventArgs changeArgs;
            if (currentMediaPresenter == mediaPresenter_alpha)
            {
                mediaPresenter.Show();
                mediaPresenter_alpha.MediaPlayer?.Stop();
                changeArgs = new MediaPlayerChangeEventArgs(mediaPresenter.MediaPlayer,mediaPresenter_alpha.MediaPlayer);
            }
            else
            {
                mediaPresenter.Hide();
                mediaPresenter.MediaPlayer?.Stop();
                changeArgs = new MediaPlayerChangeEventArgs(mediaPresenter_alpha.MediaPlayer, mediaPresenter.MediaPlayer);
            }
            MediaPlayerChangeRequestCompleted?.Invoke(this, changeArgs);
        }

        private void NewMediaPlayer_Playing(object sender, EventArgs e)
        {
            MediaPlayerPlaying?.Invoke(this, BackMediaPresenter.MediaPlayer);
        }

        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {
            MediaPlayerEndReached?.Invoke(this, EventArgs.Empty);   
        }

        /// <summary>
        /// Resizes the player to fill the screen.
        /// </summary>
        public void ResizePlayerToScreen()
        {
            APIsEncapsulated.GetScreenResolution(out int width, out int height);
            ResizePlayer(width, height);
        }

        /// <summary>
        /// Resizes the player to specified size.
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        public void ResizePlayer(int newWdith,int newHeight)
        {
            mediaPresenter.Height = newHeight + 5;
            mediaPresenter.Width = newWdith + 5;
            mediaPresenter.Left = 0;
            mediaPresenter.Top = 0;

            mediaPresenter_alpha.Height = newHeight + 5;
            mediaPresenter_alpha.Width = newWdith + 5;
            mediaPresenter_alpha.Left = 0;
            mediaPresenter_alpha.Top = 0;
        }

        private void PlayerStartup()
        {
            mediaPresenter = new VideoView();
            mediaPresenter.MediaPlayer = mediaPlayer;

            mediaPresenter_alpha = new VideoView();
            mediaPresenter_alpha.MediaPlayer = mediaPlayer;

            currentMediaPresenter = mediaPresenter;
        }

        /// <summary>
        /// Create an instance of <see cref="MediaDesktopPlayer"/> using default parameters.
        /// </summary>
        public MediaDesktopPlayer()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            PlayerStartup();
            ResizePlayerToScreen();
            EventStartup();
        }

        /// <summary>
        /// Set media to the player for playing.
        /// </summary>
        /// <param name="path">path of the media file.</param>
        public void SetMedia(string path)
        {
            Media media = new Media(libVLC, new Uri(path));
            mediaPlayer.Media = media;
        }

        /// <summary>
        /// Set media to the player for playing.
        /// </summary>
        /// <param name="path">stream of media data.</param>
        public void SetMedia(Stream stream)
        {
            Media media = new Media(libVLC, new StreamMediaInput(stream));
            mediaPlayer.Media = media;
        }

        /// <summary>
        /// Start or resume the current media playing.
        /// </summary>
        /// <returns>A boolean value that indicates the success status of this operation.</returns>
        public bool Play()
        {
            if (mediaPlayer is null)
                return false;
           return mediaPlayer.Play();
        }

        /// <summary>
        /// Pause the current media playing (no effect if there is no media or media has been paused). Some media may not support this operation.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Pause()
        {
            if (mediaPlayer is null)
                return;

            if (mediaPlayer.CanPause)
            {
                mediaPlayer.SetPause(true);
            }
            else
            {
                throw new InvalidOperationException("pausing is not supported for this media.");
            }
        }

        public bool TryPause()
        {
            Pause();
            return true;
        }

        /// <summary>
        /// Stop the current media playing. the progress of playing will be reset.This method may block current thread.
        /// </summary>
        public void Stop()
        {
            if (mediaPlayer is null)
                return;
            mediaPlayer.Stop();
        }

        /// <summary>
        /// Attach this media player to a <see cref="MediaDesktopBase"/> instance.
        /// </summary>
        /// <param name="mediaDesktopBase"></param>
        public void AttachToMediaDesktopBase(MediaDesktopBase mediaDesktopBase)
        {
            SystemAPIs.SetParent(mediaPresenter_alpha.Handle, mediaDesktopBase.AttachmentHandlerForm.Handle);
            SystemAPIs.SetParent(mediaPresenter.Handle, mediaDesktopBase.AttachmentHandlerForm.Handle);
            mediaPresenter.Show();
            mediaPresenter_alpha.Show();
        }
    }

    public class MediaPlayerChangeEventArgs
    {
        public MediaPlayerChangeEventArgs(MediaPlayer oldPlayer,MediaPlayer newPlayer)
        {
            this.OldMediaPlayer = oldPlayer;
            this.NewMediaPlayer = newPlayer;
        }

        public MediaPlayer OldMediaPlayer { get; private set; }
        public MediaPlayer NewMediaPlayer { get; private set; }
    }
}

