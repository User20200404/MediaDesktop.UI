using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Helpers
{

    class WinUI3Helper
    {
        /// <summary>
        /// Initialize target picker for using.
        /// </summary>
        /// <param name="window">An instance of <see cref="Window"/> in WinUI3.</param>
        /// <param name="targetPicker">The picker that is initialized with window.</param>
        public static void PickerInitializeWindow(Window window, object targetPicker)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(targetPicker, hwnd);
        }

        public static Size GetScreenResolution()
        {
            int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            return new Size(width, height);
        }

        public static Process SelectFileInExplorer(string filePath)
        {
            string cmd = "explorer.exe";
            string arg = "/select," + filePath;
            return Process.Start(cmd, arg);
        }

        public static Windows.UI.Color TryParseWindowsUIColor(string source, Windows.UI.Color defaultColor = default)
        {
            Windows.UI.Color def = defaultColor;
            if (source.Length != 9)
                return def;
            byte a = byte.Parse(source[1..3], System.Globalization.NumberStyles.HexNumber);
            byte r = byte.Parse(source[3..5], System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(source[5..7], System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(source[7..9], System.Globalization.NumberStyles.HexNumber);
            return Windows.UI.Color.FromArgb(a, r, g, b);
        }
    }

}
