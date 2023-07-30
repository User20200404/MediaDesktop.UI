using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using MediaDesktop.UI.ViewModels;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private ObservableCollection<SettingsNavigationItemViewModel> BreadModels
        {
            get { return Services.GlobalResources.ViewModelCollection.SettingsNavigationItemViewModels_Bread; }
        }

        public SettingsPage()
        {
            this.InitializeComponent();
            contentFrame.Navigating += ContentFrame_Navigating;
            contentFrame.Navigate(typeof(SettingsHostPage));
        }

        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            Type pageType = e.SourcePageType; //gets the previous page type.
            var model = 
                Services.GlobalResources.ViewModelCollection.SettingsNavigationItemViewModels.First(model => model.PageType == pageType);

            if (BreadModels.Contains(model))
            {
                int index = BreadModels.IndexOf(model);
                for (int i = BreadModels.Count - 1; i > index; i--)
                {
                    BreadModels.RemoveAt(i);
                }
            }
            else
            {
                BreadModels.Add(model);
            }

        }

        private void BreadCrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is SettingsNavigationItemViewModel model)
            {
                contentFrame.Navigate(model.PageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
            }
        }
    }
}
