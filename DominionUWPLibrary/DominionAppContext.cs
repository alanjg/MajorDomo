using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using Dominion.Network;
using System.Xml.Serialization;
using System.IO;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Networking;

namespace DominionUWP
{
    public class DominionAppContext
    {
		public Game CurrentGame { get; set; }
		public DifficultyModel Difficulty { get; set; }
		public GameHistory History { get; private set; }
		public bool HasLoadedGameRecords { get; set; }

		public Settings Settings { get; private set; }
		public GameSetCollection SupportedSets { get; private set; }
		public ProhibitedCardsCollection ProhibitedCards { get; private set; }
		public CardSetsModel CardSetsModel { get; set; }
		public CardCollectionModel CardCollectionModel { get; set; }
		public GameLobbyModel GameLobbyModel { get; private set; }
		public ServerConnection ServerConnection { get; set; }
		public ServerModel ServerModel { get; set; }

		public DominionAppContext()
		{
			
			this.Difficulty = new DifficultyModel();
			this.ReadHistoryFile();
			this.ReadSupportedSetsFile();
			this.ReadProhibitedCardsFile();
			this.ReadServerModelFile();
			this.ReadSettingsFile();
			this.GameLobbyModel = new GameLobbyModel();
			JupiterSocket socket = new JupiterSocket();
			this.ServerConnection = new ServerConnection(this.GameLobbyModel, socket);
		}

		public async Task DoSuspendTasks()
		{
			this.Difficulty.WriteDifficulty();
			await this.WriteHistory();
			await this.WriteSupportedSets();
			await this.WriteProhibitedCards();
			await this.WriteServerModel();
			await this.WriteSettings();
		}

		private async void ReadHistoryFile()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;

			try
			{
				StorageFile historyFile = await localFolder.GetFileAsync("history.txt");
				using (Stream stream = await historyFile.OpenStreamForReadAsync())
				{
					XmlSerializer x = new XmlSerializer(typeof(GameHistory));
					this.History = (GameHistory)x.Deserialize(stream);
				}
			}
			catch
			{
				this.History = new GameHistory();
			}
		}

		private async Task WriteHistory()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			IStorageItem item = await localFolder.TryGetItemAsync("history.txt");
			StorageFile historyFile = null;
			if (item != null)
			{
				historyFile = (StorageFile)item;
			}
			else
			{
				historyFile = await localFolder.CreateFileAsync("history.txt");
			}

			using (Stream stream = await historyFile.OpenStreamForWriteAsync())
			{
				XmlSerializer x = new XmlSerializer(typeof(GameHistory));
				x.Serialize(stream, this.History);
			}
		}

		private async void ReadSettingsFile()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;

			try
			{
				StorageFile settingsFile = await localFolder.GetFileAsync("settings.txt");
				using (Stream stream = await settingsFile.OpenStreamForReadAsync())
				{
					XmlSerializer x = new XmlSerializer(typeof(Settings));
					this.Settings = (Settings)x.Deserialize(stream);
				}
			}
			catch
			{
				this.Settings = new Settings();
			}
		}

		private async Task WriteSettings()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			IStorageItem item = await localFolder.TryGetItemAsync("settings.txt");
			StorageFile settingsFile = null;
			if (item != null)
			{
				settingsFile = (StorageFile)item;
			}
			else
			{
				settingsFile = await localFolder.CreateFileAsync("settings.txt");
			}

			using (Stream stream = await settingsFile.OpenStreamForWriteAsync())
			{
				XmlSerializer x = new XmlSerializer(typeof(Settings));
				x.Serialize(stream, this.Settings);
			}
		}


		private async void ReadSupportedSetsFile()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;

			try
			{
				StorageFile supportedSetsFile = await localFolder.GetFileAsync("supportedsets.xml");
				using (Stream stream = await supportedSetsFile.OpenStreamForReadAsync())
				{
					XmlSerializer x = new XmlSerializer(typeof(GameSetCollection));
					this.SupportedSets = (GameSetCollection)x.Deserialize(stream);
				}
			}
			catch
			{
				this.SupportedSets = new GameSetCollection();
			}

			this.CardSetsModel = new CardSetsModel(this.SupportedSets.AllowedSets);
			this.CardCollectionModel = new CardCollectionModel(this.SupportedSets.AllowedSets);
		}

		private async Task WriteSupportedSets()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			IStorageItem item = await localFolder.TryGetItemAsync("supportedsets.xml");
			StorageFile supportedSetsFile = null;
			if (item != null)
			{
				supportedSetsFile = (StorageFile)item;
			}
			else
			{
				supportedSetsFile = await localFolder.CreateFileAsync("supportedsets.xml");
			}

			using (Stream stream = await supportedSetsFile.OpenStreamForWriteAsync())
			{
				XmlSerializer x = new XmlSerializer(typeof(GameSetCollection));
				x.Serialize(stream, this.SupportedSets);
			}
		}

		private async void ReadProhibitedCardsFile()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;

			try
			{
				StorageFile supportedSetsFile = await localFolder.GetFileAsync("prohibitedcards.xml");
				using (Stream stream = await supportedSetsFile.OpenStreamForReadAsync())
				{
					XmlSerializer x = new XmlSerializer(typeof(ProhibitedCardsCollection));
					this.ProhibitedCards = (ProhibitedCardsCollection)x.Deserialize(stream);
				}
			}
			catch
			{
				this.ProhibitedCards = new ProhibitedCardsCollection();
			}
		}

		private async Task WriteProhibitedCards()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			IStorageItem item = await localFolder.TryGetItemAsync("prohibitedcards.xml");
			StorageFile supportedSetsFile = null;
			if (item != null)
			{
				supportedSetsFile = (StorageFile)item;
			}
			else
			{
				supportedSetsFile = await localFolder.CreateFileAsync("prohibitedcards.xml");
			}

			using (Stream stream = await supportedSetsFile.OpenStreamForWriteAsync())
			{
				XmlSerializer x = new XmlSerializer(typeof(ProhibitedCardsCollection));
				x.Serialize(stream, this.ProhibitedCards);
			}
		}

		private async void ReadServerModelFile()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;

			try
			{
				StorageFile serverModelFile = await localFolder.GetFileAsync("servermodel.xml");
				using (Stream stream = await serverModelFile.OpenStreamForReadAsync())
				{
					XmlSerializer x = new XmlSerializer(typeof(ServerModel));
					this.ServerModel = (ServerModel)x.Deserialize(stream);
				}
			}
			catch
			{
				this.ServerModel = new ServerModel();
			}
		}

		private async Task WriteServerModel()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			IStorageItem item = await localFolder.TryGetItemAsync("servermodel.xml");
			StorageFile serverModelFile = null;
			if (item != null)
			{
				serverModelFile = (StorageFile)item;
			}
			else
			{
				serverModelFile = await localFolder.CreateFileAsync("servermodel.xml");
			}

			using (Stream stream = await serverModelFile.OpenStreamForWriteAsync())
			{
				XmlSerializer x = new XmlSerializer(typeof(ServerModel));
				x.Serialize(stream, this.ServerModel);
			}
		}

		public void ValidateServerAddress(string address)
		{
			bool isValid = false;
			try
			{
				HostName hostName = new HostName(address);
				isValid = hostName.Type == HostNameType.Ipv4 || hostName.Type == HostNameType.Ipv6;
			}
			catch
			{
				isValid = false;
			}
			if (isValid)
			{
				this.ServerModel.ServerAddress = address;
				this.ServerModel.IsValidAddress = true;
			}
			else
			{
				this.ServerModel.ServerAddress = string.Empty;
				this.ServerModel.IsValidAddress = false;
			}
		}

		public async Task ReadHistoryRecords()
		{
			if (!this.HasLoadedGameRecords)
			{
				StorageFolder localFolder = ApplicationData.Current.LocalFolder;
				IStorageItem item = await localFolder.TryGetItemAsync("games");

				if (item != null)
				{
					StorageFolder game = await localFolder.GetFolderAsync("games");
					IReadOnlyList<StorageFile> files = await game.GetFilesAsync();
					foreach (StorageFile file in files)
					{
						using (Stream stream = await file.OpenStreamForReadAsync())
						{
							bool readFailed = false;
							XmlSerializer x = new XmlSerializer(typeof(GameRecord));
							try
							{
								GameRecord record = (GameRecord)x.Deserialize(stream);
								this.History.GameRecords.Add(record);
							}
							catch
							{
								readFailed = true;
							}
							if (readFailed)
							{
								await file.DeleteAsync();
							}
						}
					}
				}
				this.HasLoadedGameRecords = true;
			}
		}

		public async Task DeleteHistory()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			// reset history
			StorageFolder game = await localFolder.GetFolderAsync("games");
			if (game != null)
			{
				IReadOnlyList<StorageFile> files = await game.GetFilesAsync();
				foreach (StorageFile file in files)
				{
					await file.DeleteAsync();
				}
			}

			this.History.GameRecords.Clear();
			this.History.Wins = 0;
			this.History.Losses = 0;
		}
    }
}
