using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using Dominion.Network;

namespace DominionServer
{
	public enum ConnectionStatus
	{
		Unavailable,
		Ready,
		RespondingToGameProposal,
		PlayingGame,
		Exiting
	}

	public class GameMessageEventArgs : EventArgs
	{
		public NetworkMessage Message { get; set; }
	}

	public delegate void GameMessageEventHandler(GameMessageEventArgs e);

	public class RespondToGameEventArgs : EventArgs
	{
		public RespondToGameEventArgs(Connection connection, bool accepted)
		{
			this.Connection = connection;
			this.Accepted = accepted;
		}
		public bool Accepted { get; private set; }
		public Connection Connection { get; private set; }
	}

	public delegate void RespondToGameEventHandler(RespondToGameEventArgs e);

	public class Connection
	{
		public event GameMessageEventHandler GameMessage;
		public event RespondToGameEventHandler RespondedToGame;

		public ConnectionStatus Status { get; set; }
		private User user;
		public User User { get { return this.user; } }
		private Server server;
		
		private ServerSocket serverSocket;
		private Game game;
		
		private object statusLock = new object();
		public Connection(Socket socket, Server server)
		{
			this.serverSocket = new ServerSocket(socket, this);
			this.server = server;
			this.Status = ConnectionStatus.Unavailable;
			Thread t = new Thread(new ThreadStart(this.Listen));
			t.Start();			
		}

		// Called from other threads
		public void SendMessage(NetworkMessage message)
		{
			if (this.Status != ConnectionStatus.Exiting)
			{
				this.serverSocket.SendMessage(message);
				if (Server.EnableLogging)
				{
					string log = "Sending message";
					if (this.user != null && this.user.Name != null)
					{
						log += " to " + this.user.Name;
					}
					log += ": " + message;
					Console.WriteLine(log);
				}
			}
		}

		public bool TryProposeGame()
		{
			lock (this.statusLock)
			{
				if (this.Status == ConnectionStatus.Ready)
				{
					this.Status = ConnectionStatus.RespondingToGameProposal;
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public void CancelGame()
		{
			lock (this.statusLock)
			{
				if (this.Status == ConnectionStatus.RespondingToGameProposal)
				{
					this.Status = ConnectionStatus.Ready;
				}
			}
		}

		public void PlayGame(Game g)
		{
			lock (this.statusLock)
			{
				this.game = g;
				Debug.Assert(this.game == g && this.Status == ConnectionStatus.RespondingToGameProposal);
				this.Status = ConnectionStatus.PlayingGame;
			}
		}

		public void EndGame()
		{
			lock (this.statusLock)
			{
				this.game = null;
				Debug.Assert(this.Status == ConnectionStatus.PlayingGame);
				this.Status = ConnectionStatus.Ready;
			}
		}

		public void HandleMessage(NetworkMessage message)
		{			
			lock (this.statusLock)
			{
				if (Server.EnableLogging)
				{
					string log = "Received";
					if (this.user != null && this.user.Name != null)
					{
						log += " from " + this.user.Name;
					}
					log += ": ";
					log += message.ToString();
					Console.WriteLine(log);
				}

				if (message.MessageCategory == SystemMessages.SystemPrefix)
				{
					switch (message.MessageType)
					{
						case SystemMessages.Connect:
							{
								ConnectInfo connectInfo = NetworkSerializer.Deserialize<ConnectInfo>(message.MessageContent);
								if (Server.EnableLogging)
								{
									Console.WriteLine(connectInfo.Username);
								}
								this.user = this.server.AddUser(connectInfo.Username, this);
								AuthenticationInfo auth = new AuthenticationInfo();
								auth.Username = user.Name;
								auth.Authentication = user.Authentication;
								NetworkMessage authMessage = new NetworkMessage();
								authMessage.MessageCategory = SystemMessages.SystemPrefix;
								authMessage.MessageType = SystemMessages.Authenticate;
								authMessage.MessageContent = NetworkSerializer.Serialize(auth);
								this.serverSocket.SendMessage(authMessage);

								this.Status = ConnectionStatus.Ready;
							}
							break;
						case SystemMessages.SendChat:
							{
								SendChatInfo chatInfo = NetworkSerializer.Deserialize<SendChatInfo>(message.MessageContent);
								this.server.AddChat(this.user, chatInfo.Chat);
								if (Server.EnableLogging)
								{
									Console.WriteLine("Receiving chat");
								}
							}
							break;
						case SystemMessages.EnterLobby:
							{
								EnterLobbyInfo enterLobbyInfo = NetworkSerializer.Deserialize<EnterLobbyInfo>(message.MessageContent);
								Lobby lobby = null;
								if (this.server.Lobbies.TryGetValue(enterLobbyInfo.Lobby, out lobby))
								{
									this.server.AddUserToLobby(this.user, lobby);
								}

							}
							break;
						case SystemMessages.Disconnect:
							{
								this.server.RemoveUserFromLobby(this.user, this.user.Lobby);
								this.server.RemoveUser(this.user);
								this.Status = ConnectionStatus.Exiting;
								if (Server.EnableLogging)
								{
									Console.WriteLine("Exiting");
								}
							}
							break;
						case SystemMessages.RequestGame:
							{
								GameSpecificationInfo gameSpecificationInfo = NetworkSerializer.Deserialize<GameSpecificationInfo>(message.MessageContent);
								this.server.ProposeGameWithUsers(this, gameSpecificationInfo.Players, gameSpecificationInfo, this.user.Lobby);
								if (Server.EnableLogging)
								{
									Console.WriteLine("Proposing game with " + message.MessageContent);
								}
							}
							break;
						case SystemMessages.AcceptGame:
							{
								if (this.RespondedToGame != null)
								{
									this.RespondedToGame(new RespondToGameEventArgs(this, true));
								}
							}
							break;
						case SystemMessages.DeclineGame:
							{
								if (this.RespondedToGame != null)
								{
									this.RespondedToGame(new RespondToGameEventArgs(this, false));
								}
							}
							break;
					}
				}
				else if (message.MessageCategory == GameMessages.GamePrefix)
				{
					if (this.GameMessage != null)
					{
						this.GameMessage(new GameMessageEventArgs() { Message = message });
					}
				}
			}
		}

		private void Listen()
		{
			try
			{
				if (Server.EnableLogging)
				{
					Console.WriteLine("Listening on new thread");
				}
				while (this.Status != ConnectionStatus.Exiting)
				{
					this.serverSocket.Listen();
				}
				if (Server.EnableLogging && this.user != null)
				{
					Console.WriteLine(this.user.Name + " disconnected");
				}
				if (this.user != null)
				{
					this.server.RemoveUser(user);
					this.server.CloseConnection(user.Connection);
				}
				this.serverSocket.Close();
				
			}
			catch(Exception e)
			{
				string userString = this.user != null ? this.user.Name : "<no user>";
				Console.WriteLine("Exception on listener for user " + userString + ": " + e.GetType());
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);				
			}
			finally
			{
				if (this.user != null)
				{
					if (user.Lobby != null)
					{
						this.server.RemoveUserFromLobby(user, user.Lobby);
					}
					this.server.RemoveUser(user);
					this.server.CloseConnection(user.Connection);
				}
				this.serverSocket.Close();
			}
		}

		public void SendPileState(string pileName, string propertyName, string value)
		{
			PileStateInfo pileState = new PileStateInfo();
			pileState.PileName = pileName;
			pileState.PropertyName = propertyName;
			pileState.Value = value;
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.PileState;
			message.MessageContent = NetworkSerializer.Serialize(pileState);
			this.SendMessage(message);
		}

		public void SendPlayerState(int playerIndex, string propertyName, string value)
		{
			PlayerStateInfo playerState = new PlayerStateInfo();
			playerState.PlayerIndex = playerIndex;
			playerState.PropertyName = propertyName;
			playerState.Value = value;
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.PlayerState;
			message.MessageContent = NetworkSerializer.Serialize(playerState);
			this.SendMessage(message);
		}

		public void SendGameState(string propertyName, string value)
		{
			GameStateInfo gameState = new GameStateInfo();
			gameState.PropertyName = propertyName;
			gameState.Value = value;
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.GameState;
			message.MessageContent = NetworkSerializer.Serialize(gameState);
			this.SendMessage(message);
		}

		public void SendCardState(int playerIndex, string cardPileName, List<string> cards)
		{
			CardPileStateInfo cardPileState = new CardPileStateInfo();
			cardPileState.PlayerIndex = playerIndex;
			cardPileState.CardPileName = cardPileName;
			cardPileState.Cards = cards;
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.CardPileState;
			message.MessageContent = NetworkSerializer.Serialize(cardPileState);
			this.SendMessage(message);
		}
	}
}
