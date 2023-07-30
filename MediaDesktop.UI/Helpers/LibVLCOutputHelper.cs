using LibVLCSharp.Shared;
using MediaDesktop.UI.Views.Windows;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using WinRT;

namespace MediaDesktop.UI.Helpers
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public class LibVLCOutputHelper
    {
        MediaPlayer MediaPlayer;
        uint width, height, pitch;

        SoftwareBitmap softwareBitmap;
        BitmapPlaneDescription bufferLayout;
        SoftwareBitmapSource softwareBitmapSource_alpha;
        SoftwareBitmapSource softwareBitmapSource_beta;
        SoftwareBitmapSource foregroundSource;
        unsafe byte* rawDataPtr;
        uint capacity;

        public uint ActualWidth => width;
        public uint ActualHeight => height;
        public uint ActualPitch => pitch;
        public LibVLCOutputHelper(MediaPlayer unusedMediaPlayer, uint requestedWidth, uint requestedHeight)
        {
            MediaPlayer = unusedMediaPlayer;
            unusedMediaPlayer.SetVideoCallbacks(LockVideo, null, DisplayVideoAsync);
            width = Align(requestedWidth);
            height = Align(requestedHeight);
            pitch = width * 4;

            unusedMediaPlayer.SetVideoFormat("BGRA", width, height, pitch);

            softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, (int)width, (int)height, BitmapAlphaMode.Premultiplied);
            BitmapBuffer bitmapBuffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Write);

            IMemoryBufferReference memoryRef = bitmapBuffer.CreateReference();
            IMemoryBufferByteAccess memoryAccess = memoryRef.As<IMemoryBufferByteAccess>();
            bufferLayout = bitmapBuffer.GetPlaneDescription(0);
            unsafe
            {
                memoryAccess.GetBuffer(out rawDataPtr, out capacity);
            }

            softwareBitmapSource_alpha = new SoftwareBitmapSource();
            softwareBitmapSource_beta = new SoftwareBitmapSource();
            foregroundSource = softwareBitmapSource_alpha;
            bitmapBuffer.Dispose();
            memoryRef.Dispose();
        }

        private uint Align(uint value)
        {
            if (value % 32 == 0)
                return value;

            return ((value / 32) + 1) * 32;
        }

        private unsafe IntPtr LockVideo(IntPtr userdata, IntPtr planes)
        {
            Marshal.WriteIntPtr(planes, (IntPtr)rawDataPtr + bufferLayout.StartIndex);
            return userdata;
        }
        private void DisplayVideoAsync(IntPtr userdata, IntPtr picture)
        {
            ClientWindow.Instance.DispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    if (foregroundSource == softwareBitmapSource_alpha)
                    {
                        await softwareBitmapSource_beta.SetBitmapAsync(softwareBitmap);
                        ClientWindow.Instance.bitmap.Source = softwareBitmapSource_beta;
                        foregroundSource = softwareBitmapSource_beta;
                    }
                    else
                    {
                        await softwareBitmapSource_alpha.SetBitmapAsync(softwareBitmap);
                        ClientWindow.Instance.bitmap.Source = softwareBitmapSource_alpha;
                        foregroundSource = softwareBitmapSource_alpha;
                    }
                }
                catch (Exception ex) { }
            });
        }
    }

}
