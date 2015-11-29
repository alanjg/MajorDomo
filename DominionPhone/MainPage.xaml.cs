using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using DominionPhone.Resources;
using Dominion;
using System.Threading;
using Dominion.Network;

namespace DominionPhone
{
	public partial class MainPage : PhoneApplicationPage
	{
		// Constructor
		public MainPage()
		{
			InitializeComponent();

			// Sample code to localize the ApplicationBar
			//BuildLocalizedApplicationBar();
		}

		private void newSinglePlayerGamePreset(object sender, RoutedEventArgs e)
		{
			App.RootFrame.Navigate(new Uri("/CardSetChooserPage.xaml", UriKind.Relative));
		}

		private void newSinglePlayerGameCustom(object sender, RoutedEventArgs e)
		{
			App.RootFrame.Navigate(new Uri("/KingdomPickerPage.xaml", UriKind.Relative));
		}

		private void newSinglePlayerGameRandom(object sender, RoutedEventArgs e)
		{
			if (App.CurrentGame != null)
			{
				App.CurrentGame.CancelGame();
			}

			Game game = new LocalGame();
			App.CurrentGame = game;
			Kingdom kingdom = new Kingdom(null, App.ProhibitedCards.ProhibitedCards, null, App.SupportedSets.AllowedSets, 2, App.Settings.UseColonies, App.Settings.UseShelters, App.Settings.StartingHandType, App.Settings.UseRandomCardsFromChosenSetsOnly);
			game.GamePageModel.Kingdom = kingdom;
			game.PlayGame();
			App.RootFrame.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
		}

		private void newMultiPlayerGame(object sender, RoutedEventArgs e)
		{
			if (App.CurrentGame != null)
			{
				App.CurrentGame.CancelGame();
			}
			string address = !string.IsNullOrEmpty(App.ServerModel.ServerAddress) ? App.ServerModel.ServerAddress : ServerModel.PublicServerAddress;
			string username = App.ServerModel.UserName;
			App.ServerConnection.Connect(address, username);

			App.RootFrame.Navigate(new Uri("/GameLobbyPage.xaml", UriKind.Relative));
		}

		private void resumeGame(object sender, RoutedEventArgs e)
		{
			if (App.CurrentGame != null && !App.CurrentGame.GamePageModel.GameViewModel.GameOver)
			{
				App.RootFrame.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
			}
		}

		private void settings(object sender, RoutedEventArgs e)
		{
			App.RootFrame.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
		}

		private void about(object sender, RoutedEventArgs e)
		{
			App.RootFrame.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
		}

		private void records(object sender, RoutedEventArgs e)
		{
			App.RootFrame.Navigate(new Uri("/RecordsPage.xaml", UriKind.Relative));
		}		

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (App.CurrentGame != null && !App.CurrentGame.GamePageModel.GameViewModel.GameOver)
			{
				this.ResumeGameButton.Visibility = Visibility.Visible;
			}
			else
			{
				this.ResumeGameButton.Visibility = Visibility.Collapsed;
			}

			if (App.ServerModel.HasEnabledMultiplayer)
			{
				this.MultiplayerGameButton.Visibility = Visibility.Visible;
			}
			else
			{
				this.MultiplayerGameButton.Visibility = Visibility.Collapsed;
			}
			base.OnNavigatedTo(e);
		}

		// Sample code for building a localized ApplicationBar
		//private void BuildLocalizedApplicationBar()
		//{
		//    // Set the page's ApplicationBar to a new instance of ApplicationBar.
		//    ApplicationBar = new ApplicationBar();

		//    // Create a new button and set the text value to the localized string from AppResources.
		//    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
		//    appBarButton.Text = AppResources.AppBarButtonText;
		//    ApplicationBar.Buttons.Add(appBarButton);

		//    // Create a new menu item with the localized string from AppResources.
		//    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
		//    ApplicationBar.MenuItems.Add(appBarMenuItem);
		//}
	}
}