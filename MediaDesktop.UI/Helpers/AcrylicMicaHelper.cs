using Microsoft.UI.Xaml;
using System.Runtime.InteropServices; // For DllImport
using WinRT; // required to support Window.As<ICompositionSupportsSystemBackdrop>()


namespace MediaDesktop.UI.Helpers
{
    class WindowsSystemDispatcherQueueHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

        object m_dispatcherQueueController = null;
        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
            {
                // one already exists, so we'll just use it.
                return;
            }

            if (m_dispatcherQueueController == null)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;    // DQTYPE_THREAD_CURRENT
                options.apartmentType = 2; // DQTAT_COM_STA

                _ = CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
            }
        }
    }

    public class AcrylicMicaHelper
    {
        Window m_window;
        WindowsSystemDispatcherQueueHelper m_wsdqHelper = new(); // See separate sample below for implementation
        Microsoft.UI.Composition.SystemBackdrops.MicaController m_micaController = new();
        Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController m_acrylicController = new();
        Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource = new();

        public float TintOpacity { get { return m_micaController.TintOpacity; } set { m_micaController.TintOpacity = value; m_acrylicController.TintOpacity = value; } }
        public Windows.UI.Color TintColor { get { return m_micaController.TintColor; } set { m_micaController.TintColor = value; m_acrylicController.TintColor = value; } }
       // public Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration ConfigurationSource => m_configurationSource;
        public float LuminosityOpacity
        {
            get { return m_micaController.LuminosityOpacity; }
            set
            {
                m_micaController.LuminosityOpacity = value;
                m_acrylicController.LuminosityOpacity = value;
            }
        }

        public bool TrySetAcrylicBackdrop()
        {
            TryUnSetMicaAcrylicStyle();
            if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
            {
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_window.Activated += Window_Activated;
                m_window.Closed += Window_Closed;
                ((FrameworkElement)m_window.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_acrylicController.AddSystemBackdropTarget(m_window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Acrylic is not supported on m_window system
        }
        public bool TrySetMicaBackdrop()
        {
            TryUnSetMicaAcrylicStyle();
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_window.Activated += Window_Activated;
                m_window.Closed += Window_Closed;
                ((FrameworkElement)m_window.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_micaController.AddSystemBackdropTarget(m_window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on m_window system
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if(m_configurationSource is not null)
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use m_window closed window.
            if (m_acrylicController != null)
            {
                m_acrylicController.Dispose();
                m_acrylicController = null;
            }
            m_window.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)m_window.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }

        public bool TryUnSetMicaAcrylicStyle()
        {
            if (m_acrylicController != null)
            {
                m_acrylicController.RemoveAllSystemBackdropTargets();
                m_micaController.RemoveAllSystemBackdropTargets();
                return true;
            }
            return false;
        }

        public AcrylicMicaHelper(Window targetWindow)
        {
            m_window = targetWindow;
        }
    }
}