using Dominion;
using Dominion.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage;
using Windows.System.Threading;

namespace DominionJupiter
{
	public abstract class Game
	{
		public DominionAppContext Context { get; protected set; }
		public GamePageModel GamePageModel { get; protected set; }
		public abstract void PlayGame();
		public abstract void CancelGame();
		public abstract void ExitGame();

		public event EventHandler ExitingGame;
		protected void OnExitingGame()
		{
			if (this.ExitingGame != null)
			{
				this.ExitingGame(this, EventArgs.Empty);
			}
		}
	}

	public class LocalGame : Game
	{
		private IAsyncAction gameThread;
		public LocalGame(DominionAppContext context)
		{
			this.Context = context;
			LocalGamePageModel gamePageModel = new LocalGamePageModel();
			this.GamePageModel = gamePageModel;
			gamePageModel.Difficulty = this.Context.Difficulty.GetDifficulty();
		}

		public override void PlayGame()
		{
			this.GamePageModel.SetupGame();
			this.gameThread = Windows.System.Threading.ThreadPool.RunAsync(new WorkItemHandler(gameThreadStart));
		}

		private void gameThreadStart(IAsyncAction action)
		{
			this.GamePageModel.GameViewModel.GameModel.ShowPlayerScoreInDetails = this.Context.Settings.ShowPlayerScore;
			this.GamePageModel.PlayGame();
			this.WriteGameResult();
			this.Context.CurrentGame = null;
		}
		private async Task WriteGameResult()
		{
			GameResult result = this.GamePageModel.GameViewModel.GameModel.Result;
			if (result != null)
			{
				if (result.Winners.Contains(this.GamePageModel.PlayerViewModel.PlayerModel))
				{
					this.Context.History.Wins++;
				}
				else
				{
					this.Context.History.Losses++;
				}
				GameRecord record = result.ToGameRecord(this.GamePageModel.PlayerViewModel.PlayerModel);
				XmlSerializer x = new XmlSerializer(typeof(GameRecord));
				string filename = DateTime.Now.ToFileTimeUtc().ToString();

				StorageFolder localFolder = ApplicationData.Current.LocalFolder;
				StorageFolder gamesFolder = null;
				IStorageItem gamesItem = await localFolder.TryGetItemAsync("games");
				if (gamesItem == null)
				{
					gamesFolder = await localFolder.CreateFolderAsync("games");
				}
				else
				{
					gamesFolder = (StorageFolder)gamesItem;
				}

				StorageFile file = await gamesFolder.CreateFileAsync(filename + ".txt");
				using (Stream stream = await file.OpenStreamForWriteAsync())
				{
					x.Serialize(stream, record);
				}
				if (this.Context.HasLoadedGameRecords)
				{
					this.Context.History.GameRecords.Add(record);
				}
			}
		}

		public override void CancelGame()
		{
			if (this.gameThread != null)
			{
				this.gameThread.Cancel();
			}
		}

		public override void ExitGame()
		{
			this.Context.CurrentGame = null;

			this.OnExitingGame();
		}
	}

	public class RemoteGame : Game
	{
		public RemoteGame(DominionAppContext context)
		{
			this.Context = context;
			this.GamePageModel = new RemoteGamePageModel();
		}

		public override void PlayGame()
		{
			this.GamePageModel.SetupGame();
			this.Context.ServerConnection.SetGameViewModel(this.GamePageModel.GameViewModel);

			if (this.Context.ServerConnection.Player != null)
			{
				((RemoteGamePageModel)this.GamePageModel).OnSetupComplete(this.Context.ServerConnection);
			}
			else
			{
				this.Context.ServerConnection.SetupComplete += this.connection_SetupComplete;
			}
		}

		private void connection_SetupComplete(object sender, EventArgs e)
		{
			this.Context.ServerConnection.SetupComplete -= this.connection_SetupComplete;
			((RemoteGamePageModel)this.GamePageModel).OnSetupComplete(this.Context.ServerConnection);
		}

		public override void CancelGame()
		{
			this.Context.ServerConnection.Close();
		}

		public override void ExitGame()
		{
			this.OnExitingGame();
			EnterLobbyInfo enterLobby = new EnterLobbyInfo();
			enterLobby.Lobby = this.Context.GameLobbyModel.LobbyName;
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.EnterLobby;
			message.MessageContent = NetworkSerializer.Serialize(enterLobby);
			this.Context.ServerConnection.SendSystemMessage(message);
		}
	}
}
