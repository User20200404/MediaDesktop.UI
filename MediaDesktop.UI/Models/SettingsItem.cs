using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Models
{
    public class SettingsItem
    {
        #region Storage
        public string MediaItemRecordINIPath { get; set; }
        public string MediaPlayingListINIDir { get; set; }
        public string ExceptionLogPath { get; set; }
        #endregion
        #region PlayBack Control
        public int Volume { get; set; }
        public float SpeedRatio { get; set; }
        public PlayBackMode PlayBackMode { get; set; }
        #endregion
        #region Other System Configs
        public int LastLibraryPagePivotIndex { get; set; }
        public float LibraryItemScale { get; set; }
        #endregion
        #region LibVLC Configs
        public bool EnableHardwareDecoding { get; set; }

        public int FileCaching { get; set; }
        #endregion
        #region Personalization
        public Helpers.Extensions.WindowBackdropStyle AcrylicMicaStyle { get; set; }
        public Windows.UI.Color TintColor { get; set; }
        public float LuminosityOpacity { get; set; }
        public float TintOpacity { get; set; }
        public bool IsNavigationViewMaterialEnabled { get; set; }
        public bool IsPlayBackControlMaterialEnabled { get; set; }
        public bool IsPageMaterialEnabled { get; set; }
        public bool IsTitleBarMaterialEnabled { get; set; }
        #endregion
    }

    public enum PlayBackMode
    {
        Shuffle = 0,
        RepeatOne = 1
    }
}
