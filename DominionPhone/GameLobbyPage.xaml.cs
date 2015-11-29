using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Dominion.Network;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DominionPhone
{
	public partial class GameLobbyPage : PhoneApplicationPage
	{
		public GameLobbyModel Model { get { return App.GameLobbyModel; } }
		 
		public GameLobbyPage()
		{
			InitializeComponent();
			this.DataContext = this.Model;
			this.Model.ServerGameStarted += Model_ServerGameStarted;
		}

		private void Model_ServerGameStarted(object sender, EventArgs e)
		{
			this.Model.ServerGameStarted -= Model_ServerGameStarted;
			Game game = new RemoteGame();
			App.CurrentGame = game;
			game.PlayGame();
			App.RootFrame.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
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

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			if(e.Uri.OriginalString.Contains("MainPage.xaml"))
			{
				App.ServerConnection.Disconnect();
			}
		}
	}
}