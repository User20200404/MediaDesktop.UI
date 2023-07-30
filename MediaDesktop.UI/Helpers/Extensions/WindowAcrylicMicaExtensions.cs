using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Helpers.Extensions
{
    public enum WindowBackdropStyle
    {
        None = 0,
        Acrylic = 1,
        Mica = 2
    }

    /// <summary>
    /// Provides flags for recording last operation on forcing brush to update.
    /// </summary>
    enum LuminosityAdjustmentOperation
    {
        Plus = 1,
        Minus = 2
    }

    public static class WindowAcrylicMicaExtensions
    {
        private static readonly Dictionary<Window, AcrylicMicaHelper> existingPairs = new();
        private static WindowBackdropStyle lastStyle = WindowBackdropStyle.None;
        private static LuminosityAdjustmentOperation lastOperation = LuminosityAdjustmentOperation.Minus;
        public static void SetBackdropStyle(this Window window, WindowBackdropStyle style, float tintOpacity = 0f, float luminosityOpacity = 0f, Windows.UI.Color? color = null)
        {
            AcrylicMicaHelper helper;

            //Use existing helper for target window, or create new one.
            if (!existingPairs.TryGetValue(window, out helper))
            {
                helper = new AcrylicMicaHelper(window);
                existingPairs.Add(window, helper);
            }

            if (IsLastWindowBackdropStyleChanged())
            {
                bool temp = style switch
                {
                    WindowBackdropStyle.None => helper.TryUnSetMicaAcrylicStyle(),
                    WindowBackdropStyle.Acrylic => helper.TrySetAcrylicBackdrop(),
                    WindowBackdropStyle.Mica => helper.TrySetMicaBackdrop(),
                    _ => throw new InvalidOperationException("Value does not fall in expected range.")
                };
            }
            //Intial values for acrylicmicahelper.

            helper.TintOpacity = tintOpacity;

            if (luminosityOpacity < 0.001F)
                luminosityOpacity = 0.001F;
            if (luminosityOpacity > 0.999F)
                luminosityOpacity = 0.999F;

            helper.LuminosityOpacity = 1 - luminosityOpacity;

            //There is a bug where material render will not update unless LuminosityOpacity changed. We slightly increase or decrease (determined by last operation) it every time call this method.
            if (lastOperation == LuminosityAdjustmentOperation.Minus)
                helper.LuminosityOpacity += 0.001F;
            else helper.LuminosityOpacity -= 0.001F;

            if (color.HasValue && color.Value != helper.TintColor)
            {
                helper.TintColor = color.Value;
            }
            lastStyle = style;


            bool IsLastWindowBackdropStyleChanged() => lastStyle != style;
        }
    }
}
