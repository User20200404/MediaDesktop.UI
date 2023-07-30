using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Models
{
    /// <summary>
    /// Provides functions for image caching.
    /// </summary>
    public class ImageCache
    {
        public string ImagePath { get; set; }
        public static ImageSource DefaultSource { get; set; } = null;
        public ImageSource Image { get; private set; }

        /// <summary>
        /// Loads the image specified in <see cref="ImagePath"/>. If <see cref="Image"/> is already loaded, this method does nothing.
        /// </summary>
        public void TryLoadImage()
        {
            if (Image != DefaultSource && Image != null)
                return;

            TryReloadImage();
        }

        /// <summary>
        /// Loads the image specified in <see cref="ImagePath"/>. If <see cref="Image"/> is already loaded, this method overrides it. <see cref="Image"/> will be set to null if failed. Used in first call to <see cref="TryLoadImage"/>
        /// </summary>
        public void TryReloadImage()
        {
            try
            {
                BitmapImage image = new BitmapImage(new Uri(ImagePath));
                Image = image;
            }
            catch
            {
                Image = DefaultSource;
                //[LOG]
                return;
            }
        }
    }
}
