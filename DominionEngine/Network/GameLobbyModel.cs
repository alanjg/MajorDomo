using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class GameLobbyModel : NotifyingObject
	{
		public event EventHandler ServerGameStarted;

		private ServerConnection connection;
		public ServerConnection Connection { get { return this.connection; } }
		public GameLobbyModel()
		{
			this.Users = new ObservableCollection<string>();
			this.Chat = new ObservableCollection<string>();
			this.Lobbies = new ObservableCollection<string>();
			this.ShowRequestGameButton = true;
			this.ShowAcceptGameButton = false;
			this.ShowDeclineGameButton = false;
			this.ShowCancelGameButton = false;
			this.IsProcessingGameInvite = false;
			this.GameInviteStatusText = string.Empty;
		}

		public void HookServerConnection(ServerConnection connection)
		{
			this.connection = connection;
		}

		public ObservableCollection<string> Users { get; private set; }
		public ObservableCollection<string> Chat { get; private set; }
		public string LobbyName { get; set; }
		public ObservableCollection<string> Lobbies { get; private set; }

		private string gameInviteStatusText;
		public string GameInviteStatusText
		{
			get { return this.gameInviteStatusText; }
			set { this.gameInviteStatusText = value; this.OnPropertyChanged("GameInviteStatusText"); }
		}

		private bool isProcessingGameInvite;
		public bool IsProcessingGameInvite
		{
			get { return this.isProcessingGameInvite; }
			set { this.isProcessingGameInvite = value; this.OnPropertyChanged("IsProcessingGameInvite"); }
		}

		private bool showRequestGameButton;
		public bool ShowRequestGameButton
		{
			get { return this.showRequestGameButton; }
			set { this.showRequestGameButton = value; this.OnPropertyChanged("ShowRequestGameButton"); }
		}

		private bool showAcceptGameButton;
		public bool ShowAcceptGameButton
		{
			get { return this.showAcceptGameButton; }
			set { this.showAcceptGameButton = value; this.OnPropertyChanged("ShowAcceptGameButton"); }
		}

		private bool showDeclineGameButton;
		public bool ShowDeclineGameButton
		{
			get { return this.showDeclineGameButton; }
			set { this.showDeclineGameButton = value; this.OnPropertyChanged("ShowDeclineGameButton"); }
		}

		private bool showCancelGameButton;
		public bool ShowCancelGameButton
		{
			get { return this.showCancelGameButton; }
			set { this.showCancelGameButton = value; this.OnPropertyChanged("ShowCancelGameButton"); }
		}

		private bool isVisible;
		public bool IsVisible
		{
			get { return this.isVisible; }
			set { this.isVisible = value; this.OnPropertyChanged("IsVisible"); }
		}

		private bool isWaitingForGameToStart;
		public bool IsWaitingForGameToStart
		{
			get { return this.isWaitingForGameToStart; }
			set { this.isWaitingForGameToStart = value; this.OnPropertyChanged("IsWaitingForGameToStart"); }
		}

		private string gameParameters;
		public string GameParameters
		{
			get { return this.gameParameters; }
			set { this.gameParameters = value; this.OnPropertyChanged("GameParameters"); }
		}

		public void SendChat(string text)
		{
			this.connection.SendChat(text);
		}

		public void RequestGame(IEnumerable<string> selectedPlayers)
		{
			GameSpecificationInfo gameSpecification = new GameSpecificationInfo();
			gameSpecification.InitiatingPlayer = this.Connection.Username;
			gameSpecification.Players.AddRange(selectedPlayers);
			gameSpecification.Players.Add(this.connection.Username);

			if(this.gameParameters != null)
			{ 
				string[] cards = this.gameParameters.Split(',');
				foreach (string entry in cards)
				{
					string card = entry.Trim();
					bool bane = false;
					bool prohibit = false;
					string cardPart = null;
					if (card.StartsWith("bane:", StringComparison.OrdinalIgnoreCase))
					{
						bane = true;
						cardPart = card.Substring(5).Trim();
					}
					else if (card.StartsWith("!"))
					{
						prohibit = true;
						cardPart = cardPart.Substring(1).Trim();
					}
					else
					{
						cardPart = card;
					}
					if (string.Equals(cardPart, "colony", StringComparison.OrdinalIgnoreCase))
					{
						gameSpecification.UseColonies = prohibit ? CardUseType.DoNotUse : CardUseType.Use;
					}
					if (string.Equals(cardPart, "shelters", StringComparison.OrdinalIgnoreCase))
					{
						gameSpecification.UseShelters = prohibit ? CardUseType.DoNotUse : CardUseType.Use;
					}
					if (string.Equals(cardPart, "hand25", StringComparison.OrdinalIgnoreCase))
					{
						gameSpecification.StartingHandType = StartingHandType.FiveTwoSplit;
					}
					else if (string.Equals(cardPart, "hand34", StringComparison.OrdinalIgnoreCase))
					{
						gameSpecification.StartingHandType = StartingHandType.FourThreeSplit;
					}
					else if (string.Equals(cardPart, "handsame", StringComparison.OrdinalIgnoreCase))
					{
						gameSpecification.StartingHandType = StartingHandType.RandomSameStartingHands;
					}

					CardModel c = CardModelFactory.GetCardModel(card);
					if (c != null && c.IsKingdomCard)
					{
						if (bane)
						{
							gameSpecification.Bane = c.ID;
						}
						else if (prohibit)
						{
							gameSpecification.ProhibitedCards.Add(c.ID);
						}
						else if (gameSpecification.Cards.Count < 10)
						{
							gameSpecification.Cards.Add(c.ID);
						}
					}
				}
			}

			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.RequestGame;
			message.MessageContent = NetworkSerializer.Serialize(gameSpecification);
			this.connection.SendSystemMessage(message);
		}

		public void AcceptGame()
		{
			this.ShowRequestGameButton = false;
			this.ShowAcceptGameButton = false;
			this.ShowDeclineGameButton = false;
			this.ShowCancelGameButton = true;
			this.IsProcessingGameInvite = false;
			this.IsWaitingForGameToStart = true;
			this.GameInviteStatusText = string.Empty;

			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.AcceptGame;
			this.connection.SendSystemMessage(message);
			this.IsVisible = false;
		}

		public void DeclineGame()
		{
			this.ShowRequestGameButton = true;
			this.ShowAcceptGameButton = false;
			this.ShowDeclineGameButton = false;
			this.ShowCancelGameButton = false;
			this.IsProcessingGameInvite = false;
			this.GameInviteStatusText = string.Empty;

			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.DeclineGame;
			this.connection.SendSystemMessage(message);
		}

		public void CancelGame()
		{
			this.ShowRequestGameButton = true;
			this.ShowAcceptGameButton = false;
			this.ShowDeclineGameButton = false;
			this.ShowCancelGameButton = true;
			this.IsProcessingGameInvite = false;
			this.GameInviteStatusText = string.Empty;

			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.DeclineGame;
			this.connection.SendSystemMessage(message);
		}

		public void ServerProposeGame(GameSpecificationInfo gameSpecification)
		{
			this.ShowRequestGameButton = false;
			this.ShowAcceptGameButton = true;
			this.ShowDeclineGameButton = true;
			this.ShowCancelGameButton = false;
			this.IsProcessingGameInvite = true;

			string statusText = gameSpecification.InitiatingPlayer + " proposed a game with ";
			foreach (string player in gameSpecification.Players)
			{
				statusText += player + " ";
			}
			statusText += "\n";
			
			foreach (string card in gameSpecification.Cards)
			{
				statusText += card + " ";
			}
			this.GameInviteStatusText = statusText;
		}

		public void ServerCancelGame()
		{
			this.ShowRequestGameButton = true;
			this.ShowAcceptGameButton = false;
			this.ShowDeclineGameButton = false;
			this.ShowCancelGameButton = false;
			this.IsWaitingForGameToStart = false;
			this.IsProcessingGameInvite = false;
			this.GameInviteStatusText = "Game was cancelled";
		}

		public void OnServerGameStarted()
		{
			this.ShowRequestGameButton = true;
			this.ShowAcceptGameButton = false;
			this.ShowDeclineGameButton = false;
			this.ShowCancelGameButton = false;
			this.IsWaitingForGameToStart = false;
			if (this.ServerGameStarted != null)
			{
				this.ServerGameStarted(this, EventArgs.Empty);
			}
		}
	}
}
