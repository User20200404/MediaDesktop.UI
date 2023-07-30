using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Models
{
    public class SettingsNavigationItem
    {
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string Icon { get; set; }
        public string PageName { get; set; }
        public Type PageType { get; set; }
        public object PageDataContext { get; set; }

    }
}
