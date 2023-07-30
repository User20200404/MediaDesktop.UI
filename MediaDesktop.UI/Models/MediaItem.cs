using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace MediaDesktop.UI.Models
{
    public class MediaItem
    {
        public Media Media { get; set; }
        public string MediaPath { get; set; }
        public bool IsMediaLoaded { get { return Media is not null; } }

        /// <summary>
        /// Ctor.
        /// </summary>
        public MediaItem()
        {
            Media = null;
            MediaPath = "";
        }
    }
}
