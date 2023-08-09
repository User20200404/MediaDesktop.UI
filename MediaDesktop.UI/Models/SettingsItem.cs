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
        public ExtractedFramePosition ExtractedFramePosition { get; set; }
        public ExtractedFrameNamePolicy ExtractedFrameNamePolicy { get; set; }
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

    /// <summary>
    /// Represents a value that indicates where extracted frame file should be stored.
    /// </summary>
    public enum ExtractedFramePosition
    {
        /// <summary>
        /// File should be stored in the media file's source folder.
        /// </summary>
        SourceFolder = 0,
        /// <summary>
        /// File should be stored in the localcache folder distributed by the system.
        /// </summary>
        LocalCacheFolder = 1,
        /// <summary>
        /// File should be stored in the current user's photo folder.
        /// </summary>
        UserPictureFolder =2
    }

    /// <summary>
    /// Represents a value that indicates how the extracted frame file should be named.
    /// </summary>
    public enum ExtractedFrameNamePolicy
    {
        /// <summary>
        /// Appends extension to the existing media source file. e.g. ATRI.mp4 -> ATRI.mp4.png
        /// </summary>
        AppendExtension = 0,
        /// <summary>
        /// Overrides the extension of the existing media source file. e.g. ATRI.mp4 -> ATRI.png
        /// </summary>
        OverrideExtension = 1,
        /// <summary>
        /// Generates 16 random hex value for the extracted file. e.g. ATRI.mp4 -> FFEA1F04058FAC06.png
        /// </summary>
        RandomHex16 = 2,

        /// <summary>
        /// Generates 8 random hex value for the extracted file. e.g. ATRI.mp4 -> ABCDEF12.png
        /// </summary>
        RandomHex8
    }
}
