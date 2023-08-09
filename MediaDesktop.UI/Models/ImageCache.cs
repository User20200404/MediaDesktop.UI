using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace MediaDesktop.UI.Models
{
    /// <summary>
    /// Provides functions for image caching.
    /// </summary>
    public class ImageCache
    {
        public string ImagePath { get; set; }
        public static ImageSource DefaultSource { get; set; } = null;
        static MD5 MD5Hasher = MD5.Create();
        public ImageSource Image { get; private set; }
        public byte[] MD5Hash { get; private set; }

        /// <summary>
        /// Loads the image specified in <see cref="ImagePath"/>. If <see cref="Image"/> is already loaded, this method does nothing.
        /// </summary>
        public void TryLoadImage()
        {
            var hashData = TryGetMD5Hash();
            if (MD5Hash == null)
            {
                MD5Hash = hashData;
                TryReloadImage();
            }
            else
            {
                if(hashData != null && !hashData.SequenceEqual(MD5Hash))
                {
                    MD5Hash = hashData;
                    TryReloadImage();
                }
            }
        }

        /// <summary>
        /// Loads the image specified in <see cref="ImagePath"/>. If <see cref="Image"/> is already loaded, this method overrides it. <see cref="Image"/> will be set to null if failed. Used in first call to <see cref="TryLoadImage"/>
        /// </summary>
        public void TryReloadImage()
        {
            try
            {
                ForceUpdate();
            }
            catch
            {
                Image = DefaultSource;
                //[LOG]
                return;
            }
        }

        public void ForceUpdate()
        {
            if (Image == null) Image = new BitmapImage(new Uri(ImagePath));
            using var fileStream = File.OpenRead(ImagePath).AsRandomAccessStream();
            (Image as BitmapImage).SetSource(fileStream);
        }

        public byte[] TryGetMD5Hash()
        {
            try
            {
                using var fileStream = File.OpenRead(ImagePath);
                return MD5Hasher.ComputeHash(fileStream);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
