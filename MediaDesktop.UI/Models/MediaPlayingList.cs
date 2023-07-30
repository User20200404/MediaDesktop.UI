using MediaDesktop.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Models
{
    public class MediaPlayingList
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImagePath { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public ObservableCollection<MediaDesktopItemViewModel> MediaItems { get; set; }

        public MediaPlayingList()
        {
            Title = "";
            CoverImagePath = "";
            CreatedTime = DateTime.MinValue;
            ModifiedTime = DateTime.MinValue;
            MediaItems = new ObservableCollection<MediaDesktopItemViewModel>();
        }
    }
}
