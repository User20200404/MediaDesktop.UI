using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Models
{
    public class MediaDesktopItem
    {
        public int HistoryLevel { get; set; }
        public string MediaPath { get; set; }
        public string ImagePath { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public bool IsFavourite { get; set; }

        public MediaItem MediaItem { get; set; }


        /// <summary>
        /// Ctor.
        /// </summary>
        public MediaDesktopItem()
        {
            HistoryLevel = -1;
            MediaPath = "";
            ImagePath = "";
            Title = "";
            SubTitle = "";
            IsFavourite = false;
            MediaItem = new MediaItem();
        }
    }
}
