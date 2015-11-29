using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominion;
using Dominion.Network;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DominionJupiter
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GameLobbyPage : Page
	{
		private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
		}
			
		public GameLobbyModel Model { get { return App.Context.GameLobbyModel; } }
		
		public GameLobbyPage()
		{
			this.navigationHelper = new NavigationHelper(this);
			InitializeComponent();
			this.DataContext = this.Model;
			this.Model.ServerGameStarted += Model_ServerGameStarted;
		}

		private void Model_ServerGameStarted(object sender, EventArgs e)
		{
			this.Model.ServerGameStarted -= Model_ServerGameStarted;
			Game game = new RemoteGame(App.Context);
			App.Context.CurrentGame = game;
			game.PlayGame();
			App.RootFrame.Navigate(typeof(GamePage));
			game.ExitingGame += game_ExitingGame;
		}

		private void game_ExitingGame(object sender, EventArgs e)
		{
			App.RootFrame.Navigate(typeof(GameLobbyPage));
		}

		private void SendChat(object sender, RoutedEventArgs e)
		{
			string chat = this.ChatEntryTextBox.Text;
			this.Model.SendChat(chat);
			this.ChatEntryTextBox.Text = string.Empty;
		}

		private void RequestGame(object sender, RoutedEventArgs e)
		{
			if (this.UsersListBox.SelectedItems != null)
			{
				this.Model.RequestGame(this.UsersListBox.SelectedItems.Cast<string>());
			}
		}

		private void AcceptGame(object sender, RoutedEventArgs e)
		{
			this.Model.AcceptGame();
		}

		private void DeclineGame(object sender, RoutedEventArgs e)
		{
			this.Model.DeclineGame();
		}

		private void CancelGame(object sender, RoutedEventArgs e)
		{
			this.Model.CancelGame();
		}
	}
}
