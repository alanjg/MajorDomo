using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dominion.Network;

namespace DominionSL
{
	public partial class GameLobby : UserControl
	{
		public GameLobbyModel Model { get { return App.GameLobbyModel; } }
		public GameLobby()
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
			this.Visibility = System.Windows.Visibility.Collapsed;
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
