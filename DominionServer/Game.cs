using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using System.Threading;
using Dominion.Network;
using Dominion.CardSets;

namespace DominionServer
{
	public class Game
	{
		private Server server;
		private Lobby lobby;
		private List<Connection> accepted;
		private List<Connection> declined;
		private List<Connection> waiting;

		private object queueLock = new object();
		private ManualResetEvent resetEvent = new ManualResetEvent(true);
		private List<Connection> connections = new List<Connection>();
		private GameSpecificationInfo gameSpecification;
		public Game(IEnumerable<Connection> connections, Server server, Lobby lobby, GameSpecificationInfo gameSpecification)
		{
			this.connections.AddRange(connections);
			this.server = server;
			this.lobby = lobby;
			this.gameSpecification = gameSpecification;
		}

		public void NegotiateGame()
		{
			this.accepted = new List<Connection>();
			this.declined = new List<Connection>();
			this.waiting = new List<Connection>();
			for (int i = 0; i < connections.Count; i++)
			{
				this.connections[i].RespondedToGame += Game_RespondedToGame;
				this.waiting.Add(connections[i]);
			}
			for (int i = 0; i < connections.Count; i++)
			{
				NetworkMessage message = new NetworkMessage();
				message.MessageCategory = SystemMessages.SystemPrefix;
				message.MessageType = SystemMessages.ProposeGame;
				message.MessageContent = NetworkSerializer.Serialize(this.gameSpecification);
				this.connections[i].SendMessage(message);
			}
			do
			{
				resetEvent.Reset();
				resetEvent.WaitOne();
			} while (declined.Count() == 0 && waiting.Count() > 0);

			if (declined.Count() > 0)
			{
				for (int i = 0; i < this.connections.Count; i++)
				{
					NetworkMessage message = new NetworkMessage();
					message.MessageCategory = SystemMessages.SystemPrefix;
					message.MessageType = SystemMessages.CancelRequest;
					this.connections[i].SendMessage(message);
					this.connections[i].CancelGame();
				}
				return;
			}

			foreach (Connection c in this.connections)
			{
				this.server.RemoveUserFromLobby(c.User, this.lobby);
			}
			foreach (Connection c in this.connections)
			{
				NetworkMessage message = new NetworkMessage();
				message.MessageCategory = SystemMessages.SystemPrefix;
				message.MessageType = SystemMessages.GameStarted;
				c.SendMessage(message);
				c.PlayGame(this);
			}
			this.Play();
			foreach (Connection c in this.connections)
			{
				NetworkMessage message = new NetworkMessage();
				message.MessageCategory = GameMessages.GamePrefix;
				message.MessageType = GameMessages.GameEnded;
				c.SendMessage(message);
				c.EndGame();
			}
		}

		private void Game_RespondedToGame(RespondToGameEventArgs e)
		{
			lock (this.queueLock)
			{
				if (Server.EnableLogging)
				{
					Console.WriteLine(e.Connection.User.Name + " " + (e.Accepted ? "accepted" : "declined"));
				}
				if (e.Accepted)
				{
					waiting.Remove(e.Connection);
					accepted.Add(e.Connection);
				}
				else
				{
					waiting.Remove(e.Connection);
					accepted.Remove(e.Connection);
					declined.Add(e.Connection);
				}
				this.resetEvent.Set();
			}
		}

		public void Play()
		{
			GameModel model = new GameModel();
			foreach(Connection connection in this.connections)
			{
				model.Players.Add(new Player(connection.User.Name, new ServerPlayerStrategy(connection.User.Name, model, connection), model));
			}

			Kingdom kingdom = this.gameSpecification.ToKingdom();
			model.InitializeGameState(kingdom);

			SupplyPileInfo pileInfo = new SupplyPileInfo();
			pileInfo.Piles.AddRange(model.SupplyPiles.Select(p => p.Card.ID));

			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.SupplyPileInfo;
			message.MessageContent = NetworkSerializer.Serialize(pileInfo);
			foreach (Connection connection in this.connections)
			{
				connection.SendMessage(message);
			}

			if (model.ExtraPiles.Count > 0)
			{
				ExtraPileInfo extraPileInfo = new ExtraPileInfo();
				pileInfo.Piles.AddRange(model.ExtraPiles.Select(p => p.Card.ID));

				NetworkMessage message2 = new NetworkMessage();
				message2.MessageCategory = GameMessages.GamePrefix;
				message2.MessageType = GameMessages.ExtraPileInfo;
				message2.MessageContent = NetworkSerializer.Serialize(extraPileInfo);
				foreach (Connection connection in this.connections)
				{
					connection.SendMessage(message2);
				}
			}

			NetworkMessage playerInfoMessage = new NetworkMessage();
			playerInfoMessage.MessageCategory = GameMessages.GamePrefix;
			playerInfoMessage.MessageType = GameMessages.PlayerInfo;
			PlayerInfo playerInfo = new PlayerInfo();
			playerInfo.Players.AddRange(model.Players.Select(p => p.Name));
			playerInfoMessage.MessageContent = NetworkSerializer.Serialize(playerInfo);
			foreach (Connection connection in this.connections)
			{
				connection.SendMessage(playerInfoMessage);
			}

			ClientStateSynchronizer synchronizer = new ClientStateSynchronizer(connections, model);
			model.TurnEnded += model_TurnEnded;
			try
			{
				model.PlayGame();
			}
			catch (System.Net.Sockets.SocketException)
			{
			}
		}

		private void model_TurnEnded(object sender, EventArgs e)
		{
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.TurnEnded;
			
			foreach (Connection c in this.connections)
			{
				c.SendMessage(message);
			}
		}
	}	
}
