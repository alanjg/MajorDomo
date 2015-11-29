using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Dominion.Network;

namespace DominionServer
{
	public class Server
	{
		public static bool EnableLogging = false;
		public static void Main(string[] args)
		{
			Console.WriteLine("MajorDomo Dominion Server version " + NetworkMessage.ProtocolMajorVersion + "." + NetworkMessage.ProtocolMinorVersion);
			int port = 4502;
			if(args.Length > 0)
			{
				if (args[0] == "log")
				{
					Server.EnableLogging = true;
					Console.WriteLine("Logging enabled");
				}
			}

			Server server = new Server(port);
			Thread.Sleep(Timeout.Infinite);
		}

		private int port;
		private string host;
		private List<Connection> connections = new List<Connection>();
		private List<User> users = new List<User>();
		public Dictionary<string, Lobby> Lobbies { get; private set; }
		private object serverLock = new object();
		public Server(int port)
		{
			this.Lobbies = new Dictionary<string, Lobby>();
			this.port = port;
			this.host = Dns.GetHostName();
			this.Lobbies["General"] = new Lobby();
			Thread t = new Thread(new ThreadStart(this.Listen));
			t.Start();
			t.Join();
		}
		
		private void Listen()
		{
			Socket listenSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
			listenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, 0);
			listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, port));

			listenSocket.Listen(10);
			for (;;)
			{
				Socket acceptSocket = listenSocket.Accept();
				Connection connection = new Connection(acceptSocket, this);
				lock (this.serverLock)
				{
					this.connections.Add(connection);
				}
			}
		}

		public void CloseConnection(Connection connection)
		{
			lock (this.serverLock)
			{
				this.connections.Remove(connection);
			}
		}

		public User AddUser(string id, Connection connection)
		{
			LobbyInfo lobbyInfo = new LobbyInfo();
			User user;
			lock (this.serverLock)
			{
				while (this.users.Any(u => u.Name == id))
				{
					id += "0";
				}
				user = new User(id, connection);
				this.users.Add(user);

				lobbyInfo.Lobbies.AddRange(this.Lobbies.Keys);
			}

			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.SendLobbies;
			message.MessageContent = NetworkSerializer.Serialize(lobbyInfo);
			user.Connection.SendMessage(message);
			return user;
		}

		public void AddUserToLobby(User user, Lobby lobby)
		{
			AddRemoveUserInfo addUserInfo = new AddRemoveUserInfo();
			addUserInfo.User = user.Name;

			lock (this.serverLock)
			{
				NetworkMessage message = new NetworkMessage();
				message.MessageCategory = SystemMessages.SystemPrefix;
				message.MessageType = SystemMessages.AddUser;
				message.MessageContent = NetworkSerializer.Serialize(addUserInfo);

				foreach (User u in lobby.Users)
				{
					u.Connection.SendMessage(message);
				}

				UserInfo userInfo = new UserInfo();
				userInfo.Users.AddRange(lobby.Users.Select(u => u.Name));
				NetworkMessage message2 = new NetworkMessage();
				message2.MessageCategory = SystemMessages.SystemPrefix;
				message2.MessageType = SystemMessages.SendUsers;
				message2.MessageContent = NetworkSerializer.Serialize(userInfo);
				
				user.Connection.SendMessage(message2);

				lobby.Users.Add(user);
				user.Lobby = lobby;
			}
		}

		public void RemoveUserFromLobby(User user, Lobby lobby)
		{
			AddRemoveUserInfo removeUserInfo = new AddRemoveUserInfo();
			removeUserInfo.User = user.Name;
			lock (this.serverLock)
			{
				lobby.Users.Remove(user);
				user.Lobby = null;

				NetworkMessage message = new NetworkMessage();
				message.MessageCategory = SystemMessages.SystemPrefix;
				message.MessageType = SystemMessages.RemoveUser;
				message.MessageContent = NetworkSerializer.Serialize(removeUserInfo);

				foreach (User u in lobby.Users)
				{
					u.Connection.SendMessage(message);
				}
				UserInfo userInfo = new UserInfo();
				NetworkMessage message2 = new NetworkMessage();
				message2.MessageCategory = SystemMessages.SystemPrefix;
				message2.MessageType = SystemMessages.SendUsers;
				message2.MessageContent = NetworkSerializer.Serialize(userInfo);
				user.Connection.SendMessage(message2);
			}
		}

		public void RemoveUser(User user)
		{
			lock (this.serverLock)
			{
				this.users.Remove(user);
			}
		}

		public void AddChat(User user, string chatMsg)
		{
			ChatInfo chatInfo = new ChatInfo();
			chatInfo.Chat = chatMsg;
			chatInfo.Name = user.Name;

			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.SendChat;
			message.MessageContent = NetworkSerializer.Serialize(chatInfo);

			lock (this.serverLock)
			{
				Chat chat = new Chat(user, chatMsg, DateTime.Now);
				user.Lobby.Chats.Add(chat);
				foreach (User u in user.Lobby.Users)
				{
					u.Connection.SendMessage(message);
				}
			}
		}

		public void ProposeGameWithUsers(Connection source, List<string> players, GameSpecificationInfo gameSpecification, Lobby lobby)
		{
			List<Connection> gameConnections = new List<Connection>();
			lock (this.serverLock)
			{
				foreach (string target in players)
				{
					User user = lobby.Users.FirstOrDefault(u => u.Name == target);
					if (user == null)
					{
						NetworkMessage message = new NetworkMessage();
						message.MessageCategory = SystemMessages.SystemPrefix;
						message.MessageType = SystemMessages.DeclineGame;
						source.SendMessage(message);
						return;
					}
					else
					{
						gameConnections.Add(user.Connection);
					}
				}

				for (int i = 0; i < gameConnections.Count; i++)
				{
					bool ok = gameConnections[i].TryProposeGame();
					if (!ok)
					{
						for (int j = 0; j < i; j++)
						{
							gameConnections[j].CancelGame();
						}
						return;
					}
				}
			}
			Game game = new Game(gameConnections, this, lobby, gameSpecification);
			Thread thread = new Thread(new ThreadStart(game.NegotiateGame));
			thread.Start();
		}	
	}
}
