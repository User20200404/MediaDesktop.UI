using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDesktop.UI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace MediaDesktop.UI.ViewModels
{
    public class SettingsNavigationItemViewModel : INotifyPropertyChanged
    {
        private SettingsNavigationItem settingsNavigationItem = new SettingsNavigationItem();
        public string Title
        {
            get { return settingsNavigationItem.Title; }
            set
            {
                if (settingsNavigationItem.Title != value)
                {
                    settingsNavigationItem.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        public string Introduction
        {
            get { return settingsNavigationItem.Introduction; }
            set
            {
                if (settingsNavigationItem.Introduction != value)
                {
                    settingsNavigationItem.Introduction = value;
                    OnPropertyChanged(nameof(Introduction));
                }
            }
        }
        public string Icon
        {
            get { return settingsNavigationItem.Icon; }
            set
            {
                if (settingsNavigationItem.Icon != value)
                {
                    settingsNavigationItem.Icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }

        public string PageName
        {
            get { return settingsNavigationItem.PageName; }
            set
            {
                if (settingsNavigationItem.PageName != value)
                {
                    settingsNavigationItem.PageName = value;
                    OnPropertyChanged(nameof(PageName));
                }
            }
        }

        public Type PageType
        {
            get { return settingsNavigationItem.PageType; }
            set
            {
                if (settingsNavigationItem.PageType != value)
                {
                    settingsNavigationItem.PageType = value;
                    OnPropertyChanged(nameof(PageType));
                }
            }
        }

        public object PageDataContext
        {
            get { return settingsNavigationItem.PageDataContext; }
            set
            {
                if (settingsNavigationItem.PageDataContext != value)
                {
                    settingsNavigationItem.PageDataContext = value;
                    OnPropertyChanged(nameof(PageDataContext));
                }
            }
        }

        #region Methods
        /// <summary>
        /// Finds the nearest parent <see cref="Frame"/> element, and make it navigate to <see cref="PageType"/>
        /// </summary>
        public void ParentFrameNavigateToPage(DependencyObject element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while(parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent.GetType() == typeof(Frame))
                {
                    break;
                }
            }

            if (parent is Frame contentFrame)
            {
                contentFrame.Navigate(PageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
            }
        }
        #endregion

        #region Delegate Commands
        public DelegateCommand ParentFrameNavigateToPageDelegateCommand { get; set; }
        #endregion

        #region Inner Methods
        private void DelegateCommandStartup()
        {
            ParentFrameNavigateToPageDelegateCommand = new DelegateCommand((obj) => { ParentFrameNavigateToPage(obj as FrameworkElement); });
        }
        #endregion

        #region Ctor
        public SettingsNavigationItemViewModel()
        {
            DelegateCommandStartup();
        }
        #endregion

        #region Notify Event&Methods
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
