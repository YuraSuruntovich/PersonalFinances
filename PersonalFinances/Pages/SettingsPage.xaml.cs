using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        ApplicationDataContainer localSettings;
        ApplicationView view;
        public SettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            view = ApplicationView.GetForCurrentView();
            localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["isFullScreenMode"];

            if(value != null)
            {
                if(value.ToString() == "true")
                {
                    view.TryEnterFullScreenMode();
                    toggleSwitchFullScreen.IsOn = true;
                }
                else if(value.ToString() == "false")
                {
                    view.ExitFullScreenMode();
                    toggleSwitchFullScreen.IsOn = false;
                }
            }
        }

        private void toggleSwitchFullScreen_Toggled(object sender, RoutedEventArgs e)
        {
            view = ApplicationView.GetForCurrentView();
            localSettings = ApplicationData.Current.LocalSettings;
            
            if (toggleSwitchFullScreen.IsOn == true)
            {
                localSettings.Values["isFullScreenMode"] = "true";
                view.TryEnterFullScreenMode();
            }
            else
            {
                localSettings.Values["isFullScreenMode"] = "false";
                view.ExitFullScreenMode();
            }

        }
    }
}
