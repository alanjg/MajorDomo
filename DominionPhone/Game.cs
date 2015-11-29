using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dominion;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.Windows.Navigation;
using Dominion.Network;

namespace DominionPhone
{
	public abstract class Game
	{
		public GamePageModel GamePageModel { get; protected set; }
		public abstract void PlayGame();
		public abstract void ExitGame();
		public abstract void CancelGame();
	}

	public class LocalGame : Game
	{
		private Thread gameThread;
		public LocalGame()
		{
			LocalGamePageModel gamePageModel = new LocalGamePageModel();
			this.GamePageModel = gamePageModel;
			gamePageModel.Difficulty = App.Difficulty.GetDifficulty();		
		}

		public override void PlayGame()
		{
			this.gameThread = new Thread(new ThreadStart(this.gameThreadStart));	
			this.GamePageModel.SetupGame();
			gameThread.Start();
		}

		private void gameThreadStart()
		{
			this.GamePageModel.GameViewModel.GameModel.ShowPlayerScoreInDetails = App.Settings.ShowPlayerScore;
			this.GamePageModel.PlayGame();
			GameResult result = this.GamePageModel.GameViewModel.GameModel.Result;
			if (result != null)
			{
				if (result.Winners.Contains(this.GamePageModel.PlayerViewModel.PlayerModel))
				{
					App.History.Wins++;
				}
				else
				{
					App.History.Losses++;
				}
				GameRecord record = result.ToGameRecord(this.GamePageModel.PlayerViewModel.PlayerModel);
				XmlSerializer x = new XmlSerializer(typeof(GameRecord));
				string filename = DateTime.Now.ToFileTimeUtc().ToString();
				IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
				if (!isoStore.DirectoryExists("games"))
				{
					isoStore.CreateDirectory("games");
				}

				using (IsolatedStorageFileStream stream = isoStore.CreateFile("games\\" + filename + ".txt"))
				{
					x.Serialize(stream, record);
				}
				if (App.HasLoadedGameRecords)
				{
					App.History.GameRecords.Insert(0, record);
				}
			}
		}

		public override void CancelGame()
		{
			if (this.gameThread != null)
			{
				this.gameThread.Abort();
			}
		}

		public override void ExitGame()
		{
			App.CurrentGame = null;
			App.RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
		}
	}

	public class RemoteGame : Game
	{
		public RemoteGame()
		{
			this.GamePageModel = new RemoteGamePageModel();
		}
		
		public override void PlayGame()
		{			
			this.GamePageModel.SetupGame();
			App.ServerConnection.SetGameViewModel(this.GamePageModel.GameViewModel);
			if (App.ServerConnection.Player != null)
			{
				((RemoteGamePageModel)this.GamePageModel).OnSetupComplete(App.ServerConnection);
			}
			else
			{
				App.ServerConnection.SetupComplete += this.connection_SetupComplete;
			}
		}
		
		private void connection_SetupComplete(object sender, EventArgs e)
		{
			App.ServerConnection.SetupComplete -= this.connection_SetupComplete;
			((RemoteGamePageModel)this.GamePageModel).OnSetupComplete(App.ServerConnection);
		}

		public override void CancelGame()
		{
			App.ServerConnection.Close();
		}

		public override void ExitGame()
		{
			EnterLobbyInfo enterLobby = new EnterLobbyInfo();
			enterLobby.Lobby = App.GameLobbyModel.LobbyName;
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.EnterLobby;
			message.MessageContent = NetworkSerializer.Serialize(enterLobby);
			App.ServerConnection.SendSystemMessage(message);
			App.RootFrame.Navigate(new Uri("/GameLobbyPage.xaml", UriKind.Relative));			
		}
	}
}
