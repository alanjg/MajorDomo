using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominion;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DominionJupiter.Common;
using Dominion.Network;

namespace DominionJupiter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
	{
		private NavigationHelper navigationHelper;
      
        public MainPage()
		{
			this.navigationHelper = new NavigationHelper(this);
			this.InitializeComponent(); 
        }

		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (App.Context.CurrentGame != null && !App.Context.CurrentGame.GamePageModel.GameViewModel.GameOver)
			{
				this.ResumeGameButton.Visibility = Visibility.Visible;
			}
			else
			{
				this.ResumeGameButton.Visibility = Visibility.Collapsed;
			}

			if (App.Context.ServerModel != null && App.Context.ServerModel.HasEnabledMultiplayer)
			{
				this.MultiplayerGameButton.Visibility = Visibility.Visible;
			}
			else
			{
				this.MultiplayerGameButton.Visibility = Visibility.Collapsed;
			}
			navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		void clickSinglePlayerPreset(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(CardSetChooserPage));
		}

		void clickSinglePlayerCustom(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(KingdomPickerPage));
		}

		void clickSettings(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(SettingsPage));
		}

		void clickAbout(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(About));
		}

		void clickRecords(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(RecordsPage));
		}

		void clickSinglePlayerRandom(object sender, RoutedEventArgs e)
		{
			Kingdom kingdom = new Kingdom(null, App.Context.ProhibitedCards.ProhibitedCards, null, App.Context.SupportedSets.AllowedSets, 2, App.Context.Settings.UseColonies, App.Context.Settings.UseShelters, App.Context.Settings.StartingHandType, App.Context.Settings.UseRandomCardsFromChosenSetsOnly);
			App.StartLocalGame(kingdom);
		}

		void clickResumeGame(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(GamePage));
		}

		private void clickMultiPlayer(object sender, RoutedEventArgs e)
		{
			if (App.Context.CurrentGame != null)
			{
				App.Context.CurrentGame.CancelGame();
			}

			string address = !string.IsNullOrEmpty(App.Context.ServerModel.ServerAddress) ? App.Context.ServerModel.ServerAddress : ServerModel.PublicServerAddress;
			string username = App.Context.ServerModel.UserName;
			App.Context.ServerConnection.Connect(address, username);

			this.Frame.Navigate(typeof(GameLobbyPage));
		}
    }
}
