using MediaDesktop.UI.Models;
using MediaDesktop.UI.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.PointOfService.Provider;

namespace MediaDesktop.UI.ViewModels
{
    public class IsFavouriteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool favourite = (bool)value;
            switch (parameter)
            {
                case "Icon":
                    return ConvertTextIcon(favourite);
                case "Foreground":
                    return ConvertForeground(favourite);
                default:
                    return null;
            }
        }

        private string ConvertTextIcon(bool favourite)
        {
            if (favourite)
                return "\xEB52";
            else return "\xEB51";
        }

        private SolidColorBrush ConvertForeground(bool favourite)
        {
            if (favourite)
                return new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xFF, 0, 0));
            else return new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IsPlayingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isPlaying = (bool)value;
            if (isPlaying)
                return "\xE769";
            else return "\xE768";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MillionSecondsTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            long ms = (long)value;
            int t_H = (int)Math.Floor( ms / (float)3600000);
            long sub_H = ms - t_H * 3600000;
            int t_M = (int)Math.Floor( sub_H / (float)60000);
            long sub_M = sub_H - t_M * 60000;
            int t_S = (int)Math.Floor( sub_M / (float)1000);

            string formatString = t_H.ToString("D2") + ":" + t_M.ToString("D2") + ":" + t_S.ToString("D2");
            return formatString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SliderProgressConverter : IValueConverter
    {
        /// <summary>
        /// Convert position(0f-1f) to slider value.
        /// </summary>
        /// <param name="value">Source position(0f-1f)</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">The slider's max value. Consider using binding mark-up.</param>
        /// <param name="language">Ignored</param>
        /// <returns>Slider's value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            float sourceValue = (float)value;
            int maxValue = int.Parse(parameter as string);
            double result = (double)(maxValue * sourceValue);
            return result;
        }

        /// <summary>
        /// Convert slider value to position(0f-1f).
        /// </summary>
        /// <param name="value">Slider's current value.</param>
        /// <param name="targetType">Ignored.</param>
        /// <param name="parameter">The slider's max value. Consider using binding mark-up.</param>
        /// <param name="language">Ignored</param>
        /// <returns>Position(0f-1f).</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int sliderValue = (int)value;
            int maxValue = (int)parameter;
            float result = (float)sliderValue / (float)maxValue;
            return result;
        }
       
    }
    public class SizeToResolutionStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            System.Drawing.Size size = (System.Drawing.Size)value;
            string resolution = size.Width.ToString() + "*" + size.Height.ToString();
            return resolution;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ByteSizeToMByteSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            long sizeInBytes = (long)value;
            float sizeInMBytes = (float)sizeInBytes / (1024 * 1024);
            return sizeInMBytes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MBToKbSizeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            float size_M = (float)value;
            return size_M * 1000 * 8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class VolumeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int volume = (int)value;

            if (volume == 0)
            {
                return "\xE992";
            }
            if (volume > 0 && volume <= 33)
            {
                return "\xE993";
            }
            if (volume > 33 && volume <= 66)
            {
                return "\xE994";
            }
            if (volume > 66 && volume <= 100)
            {
                return "\xE995";
            }

            return "\xEA39";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class PlayBackModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            PlayBackMode mode = (PlayBackMode)value;
            switch (mode)
            {
                case PlayBackMode.Shuffle:
                    return "\xE8B1";
                case PlayBackMode.RepeatOne:
                    return "\xE8ED";
                default:
                    break;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class VisbileIfEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value.ToString() == parameter.ToString())
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class InvisbileIfEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value.ToString() == parameter.ToString())
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class ImageSourceOrDefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string path = value as string;
            ImageSource source;
            var cache = GlobalResources.ImageCaches.FirstOrDefault(i => i.ImagePath == path);
            if (cache is not null)
            {
                cache.TryLoadImage();
                source = cache.Image;
            }
            else
            {
                ImageCache newCache = new ImageCache() { ImagePath = path };
                newCache.TryLoadImage();
                if(newCache.Image is not null)
                {
                    GlobalResources.ImageCaches.Add(newCache);
                }
                source = newCache.Image;
            }
            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Provides converter for combobox control's twoway-binding to a enum type.
    /// </summary>
    public class SelectEnumValueComboBoxConverter : IValueConverter
    {
        public Type EnumType { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Enum.Parse(EnumType, value as string);
        }
    }


    /// <summary>
    /// Provides converter for inverting boolean value.
    /// </summary>
    public class BooleanInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
            {
                return !b;
            }
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, language);
        }
    }

    public class BasicCalculateConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, string language)
        {
            string para = parameter as string;
            char op = para[0];
            double rightVal = double.Parse(para[1..]);
            double leftVal = System.Convert.ToDouble(value);
            return op switch
            {
                '+' => leftVal + rightVal,
                '-' => leftVal - rightVal,
                '*' => leftVal * rightVal,
                '/' => leftVal / rightVal,
                '%' => leftVal % rightVal,
                _ => throw new InvalidDataException($"{nameof(BasicCalculateConverter)}: Invalid Number Or Operator.")
            };
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, language);
        }
    }

    public class BasicCalculateCornerRadiusConverter : BasicCalculateConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {  
            double baseVal = (double)base.Convert(value, targetType, parameter, language);
            return new CornerRadius(baseVal);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            CornerRadius radius = (CornerRadius)value;
            return radius.TopRight;
        }
    }


    public class FloatToVector : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            float f = (float)value; 
            Vector3 vector = new Vector3(f, f, f);
            return vector;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IsInequalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
