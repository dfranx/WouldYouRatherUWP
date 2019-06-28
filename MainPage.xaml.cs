using System;
using System.IO;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Documents;
using Windows.ApplicationModel.Core;
using System.Globalization;
using WouldYouRather;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace WouldYouRatherUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        class QuestionData
        {
            public string Title = "Title";
            public string ChoiceA = "Choice A";
            public string ChoiceB = "Choice B";
            public string Link = "www.google.com";
            public int Votes = 0;
            public bool NSFW = false;
        }

        private Color MainColor;
		private Color TextColor;
        private QuestionData Question;
        
        public MainPage()
        {
            MainColor = (Color)this.Resources["SystemAccentColor"];
            TextColor = IsThemeDark() ? Colors.White : Colors.Black;

			if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("theme"))
				ApplicationData.Current.LocalSettings.Values["theme"] = (int)ElementTheme.Default;
			if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("filter_nsfw"))
				ApplicationData.Current.LocalSettings.Values["filter_nsfw"] = true;

			this.RequestedTheme = (ElementTheme)ApplicationData.Current.LocalSettings.Values["theme"];

			this.InitializeComponent();
			
            // status bar and title bar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar sb = StatusBar.GetForCurrentView();
                sb.ShowAsync().AsTask().Wait();
				sb.BackgroundColor = (Color)this.Resources["SystemChromeMediumColor"];
                sb.ForegroundColor = TextColor;
                sb.BackgroundOpacity = 1;

                // show/hide the status bar according to orientation
                Window.Current.SizeChanged += async (sender, e) =>
                {
                    var statusBar = StatusBar.GetForCurrentView();
                    if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
                        await statusBar.HideAsync();
                    else
                        await statusBar.ShowAsync();
                };
            }
            else
            {
                ApplicationViewTitleBar tb = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;

                tb.BackgroundColor = MainColor;
                tb.ButtonBackgroundColor = Colors.Transparent;
				tb.ButtonInactiveBackgroundColor = Colors.Transparent;
				tb.ButtonForegroundColor = TextColor;
				tb.ForegroundColor = TextColor;

				CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

				// update draggable region
				Window.Current.SetTitleBar(AppTitleBar);
			}
			
			// display starting question
			DisplayNewQuestion();
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			TextColor = IsThemeDark() ? Colors.White : Colors.Black;
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
				StatusBar sb = StatusBar.GetForCurrentView();
				sb.ShowAsync().AsTask().Wait();
				sb.BackgroundColor = ((SolidColorBrush)CommandBar.Background).Color;
				sb.ForegroundColor = TextColor;
				sb.BackgroundOpacity = 1;
			} else {
				ApplicationViewTitleBar tb = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;

				tb.ButtonForegroundColor = TextColor;
				tb.ForegroundColor = TextColor;
			}

			base.OnNavigatedTo(e);
		}

		bool IsThemeDark()
        {
            return this.RequestedTheme == ElementTheme.Default ? (Application.Current.RequestedTheme == ApplicationTheme.Dark) : (this.RequestedTheme == ElementTheme.Dark);
        }
        
        async Task<String> GetQuestion()
        {
            WebRequest request = WebRequest.Create("http://rrrather.com/botapi");
            WebResponse response = request.GetResponseAsync().Result;
            Stream data = response.GetResponseStream();
            string html = String.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            return html;
        }
		
        private void DisplayNewQuestion()
        {
            Question = JsonConvert.DeserializeObject<QuestionData>(GetQuestion().Result);
			bool filter = (bool)ApplicationData.Current.LocalSettings.Values["filter_nsfw"];

			if (Question.NSFW)
				return;

			if (filter && Question.NSFW)
			{
				DisplayNewQuestion();
				return;
			}

            TextQ1.Text = Question.ChoiceA;
            TextQ2.Text = Question.ChoiceB;
            WYRTitle.Text = Question.Title;
            votes.Text = Question.Votes.ToString() + " votes";
        }
        
		private void Q1Holder_Click(object sender, RoutedEventArgs e)
		{
			DisplayNewQuestion();
		}

		private void Q2Holder_Click(object sender, RoutedEventArgs e)
		{
			DisplayNewQuestion();
		}

		private async void AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri(Question.Link));
		}

		private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(Settings));
		}
	}
}
