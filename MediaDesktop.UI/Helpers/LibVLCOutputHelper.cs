using LibVLCSharp.Shared;
using MediaDesktop.UI.Views.Windows;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
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

    public class SoftwareBitmapEventArgs : EventArgs
    {
        public SoftwareBitmap Bitmap { get; }
        public SemaphoreSlim UnlockBufferSemaphore { get; } 
        public SoftwareBitmapEventArgs(SoftwareBitmap bitmap,SemaphoreSlim semaphore) 
        {
            Bitmap = bitmap;
            UnlockBufferSemaphore = semaphore;
        }
    }

    public class LibVLCOutputHelper : IDisposable
    {
        MediaPlayer MediaPlayer;
        uint width, height, pitch;

        int lastDisplayedPictures = -1;
        byte[] lastFrameMD5Hash;
        UnmanagedMemoryStream rawDataByteStream;
        SoftwareBitmap softwareBitmap;
        BitmapPlaneDescription bufferLayout;
        unsafe byte* rawDataPtr;
        uint capacity;
        private bool disposedValue;

        public uint ActualWidth => width;
        public uint ActualHeight => height;
        public uint ActualPitch => pitch;

        public event EventHandler<SoftwareBitmap> BitmapRenderCompleted;
        public unsafe LibVLCOutputHelper(MediaPlayer unusedMediaPlayer, uint requestedWidth, uint requestedHeight)
        {
            MediaPlayer = unusedMediaPlayer;

            unusedMediaPlayer.SetVideoCallbacks(LockVideo, null, DisplayVideo);
            width = Align(requestedWidth);
            height = Align(requestedHeight);
            pitch = width * 4;

            unusedMediaPlayer.SetVideoFormat("BGRA", width, height, pitch);

            softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, (int)width, (int)height, BitmapAlphaMode.Premultiplied);
            BitmapBuffer bitmapBuffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite);

            IMemoryBufferReference memoryRef = bitmapBuffer.CreateReference();
            IMemoryBufferByteAccess memoryAccess = memoryRef.As<IMemoryBufferByteAccess>();
            bufferLayout = bitmapBuffer.GetPlaneDescription(0);
            unsafe
            {
                memoryAccess.GetBuffer(out rawDataPtr, out capacity);
            }
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

        private unsafe void DisplayVideo(IntPtr userdata, IntPtr picture)
        {
            BitmapRenderCompleted?.Invoke(this, softwareBitmap);
        }

        bool IsDisplayedPicturesIncreased()
        {
            int thisDisplayedPictures = MediaPlayer.Media.Statistics.DisplayedPictures;
            if (thisDisplayedPictures > lastDisplayedPictures)
            {
                lastDisplayedPictures = thisDisplayedPictures;
                return true;
            }
            return false;
        }

        bool IsCurrentFrameDuplicated()
        {
            byte[] thisFrameMD5Hash = ComputeHashForRawData();
            bool eq;
            if (lastFrameMD5Hash == null)
                eq = false;
            else
                eq = thisFrameMD5Hash.SequenceEqual(lastFrameMD5Hash);
            foreach (byte b in thisFrameMD5Hash)
                Debug.Write(b + " ");

            Debug.Write('\n');
            if (lastFrameMD5Hash != null)
                foreach (byte b in lastFrameMD5Hash)
                    Debug.Write(b + " ");
            Debug.Write('\n');
            lastFrameMD5Hash = thisFrameMD5Hash;
            return eq;
        }

        unsafe byte[] ComputeHashForRawData()
        {
            MD5 md5 = MD5.Create();
            rawDataByteStream.Position = 0;
            return md5.ComputeHash(rawDataByteStream);
        }

        public void SetOutputBitmapSource(SoftwareBitmapSource source, DispatcherQueue dispatcherQueue = null)
        {
            if (dispatcherQueue == null)
                dispatcherQueue = source.DispatcherQueue;
            source.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High,async () =>
            {
                if (disposedValue)
                    return;
                try {
                    await source.SetBitmapAsync(softwareBitmap);
                }
                catch { }
             });
        }

        ~LibVLCOutputHelper()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    MediaPlayer.Stop();
                    MediaPlayer.Dispose();
                    softwareBitmap.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~LibVLCOutputHelper()
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
    }

}
