using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WouldYouRather
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();

			// theme
			this.RequestedTheme = (ElementTheme)ApplicationData.Current.LocalSettings.Values["theme"];
			ThemeList.SelectedIndex = (int)this.RequestedTheme;
			ToggleNSFW.IsOn = (bool)ApplicationData.Current.LocalSettings.Values["filter_nsfw"];

			//
			Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += Settings_BackRequested;

			// status bar and title bar
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
				
			}
			else
			{
				// update draggable region
				Window.Current.SetTitleBar(AppTitleBar);
			}
		}

		private void Settings_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
		{
			Frame rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null)
				return;

			// Navigate back if possible, and if the event has not 
			// already been handled .
			if (rootFrame.CanGoBack && e.Handled == false)
			{
				e.Handled = true;
				rootFrame.GoBack();
			}
		}

		private void ThemeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.RequestedTheme = (ElementTheme)ThemeList.SelectedIndex;
			ApplicationData.Current.LocalSettings.Values["theme"] = (int)this.RequestedTheme;
		}
		
		private void ToggleNSFW_Toggled(object sender, RoutedEventArgs e)
		{
			ApplicationData.Current.LocalSettings.Values["filter_nsfw"] = ToggleNSFW.IsOn;
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.Frame.CanGoBack)
				this.Frame.GoBack();
		}
	}
}
