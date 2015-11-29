using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using DominionServer;
using Dominion;
using Dominion.Model.Actions;
using System.Diagnostics;
using Dominion.Network;

namespace TestClient
{
	class TestClient
	{
		private static Socket ConnectSocket(string server, int port)
		{
			Socket s = null;
			IPHostEntry hostEntry = null;
			hostEntry = Dns.GetHostEntry(server);
			foreach (IPAddress address in hostEntry.AddressList)
			{
				IPEndPoint ipe = new IPEndPoint(address, port);
				Socket tempSocket =
					new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				tempSocket.Connect(ipe);

				if (tempSocket.Connected)
				{
					s = tempSocket;
					break;
				}
				else
				{
					continue;
				}
			}
			return s;
		}

		private static string Receive(Socket socket)
		{
			Byte[] bytesReceived = new Byte[256];
			int bytes = 0;
			string result = string.Empty;

			do
			{
				bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
				result = result + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
			}
			while (bytes == bytesReceived.Length);
			return result;
		}
		private static Queue<string> messageQueue = new Queue<string>();
		private static string GetNextMessage(Socket socket)
		{
			if (messageQueue.Count > 0)
			{
				return messageQueue.Dequeue();
			}
			string[] messages = Receive(socket).Split('$');
			foreach (string msg in messages)
			{
				if (!string.IsNullOrEmpty(msg))
				{
					messageQueue.Enqueue(msg);
				}
			}
			if (messageQueue.Count > 0)
			{
				return messageQueue.Dequeue();
			}
			else
			{
				return string.Empty;
			}
		}

		private static void Send(string message, Socket socket)
		{
			Byte[] bytesSent = Encoding.ASCII.GetBytes(message);
			socket.Send(bytesSent, bytesSent.Length, 0);
		}

		public static void Main(string[] args)
		{
			int port = 4502;

			string host = Dns.GetHostName();

			Socket s = ConnectSocket(host, port);

			Console.Write("Enter id:");
			string id = Console.ReadLine();

			Send("CONNECT " + id, s);
			string result = GetNextMessage(s);
			Console.WriteLine(result);
			string authentication = result.Substring(13);
			string command;
			DateTime lastChat = DateTime.Now;
			GameModel gameModel = new GameModel();
			ServerStateSynchronizer synchronizer = new ServerStateSynchronizer(gameModel);
			do
			{
				Console.WriteLine("Command(sendchat, viewchat, users, game, quit, accept, decline, check):");
				command = Console.ReadLine();
				if (command.StartsWith("sendchat"))
				{
					string chat = command.Substring(9);
					Send("SYSTEM|SENDCHAT " + chat, s);
				}
				else if (command.StartsWith("viewchat"))
				{
					Send("SYSTEM|GETCHAT " + lastChat.ToString(), s);
					lastChat = DateTime.Now;
					string chat = GetNextMessage(s);
					// starts with GETCHAT
					if (chat != "NOCHAT")
					{
						Console.WriteLine(chat.Substring(8));
					}
				}
				else if (command.StartsWith("users"))
				{
					Send("SYSTEM|GETUSERS", s);
					string users = GetNextMessage(s);
					Console.WriteLine(users);
				}
				else if (command.StartsWith("game"))
				{
					Send("SYSTEM|REQUESTGAME" + command.Substring(4), s); 
				}
				else if(command.StartsWith("check"))
				{
					string cmd = GetNextMessage(s);
					Console.WriteLine(cmd);
				}
				else if(command.StartsWith("decline"))
				{
					Send("SYSTEM|DECLINEGAME", s);
				}
				else if (command == "accept")
				{
					Send("SYSTEM|ACCEPTGAME", s);
					//string conn = GetNextMessage(s);
					string conn = "";
					//Console.WriteLine("GOT: " + conn);
					while (conn != GameMessages.GamePrefix + GameMessages.GameEnded)
					{						
						if (conn.StartsWith("GAME|"))
						{
							string msg = conn.Substring(5);
							if (msg == GameMessages.RequestAction)
							{
								PrintClientState(gameModel, id);
								Console.WriteLine("waiting for action");
								string req = Console.ReadLine();
								Send("GAME|" + req, s);
							}
							else if (msg.StartsWith(GameMessages.RequestChoice))
							{
								int q1 = msg.IndexOf('\"') + 1;
								int q2 = msg.IndexOf('\"', q1);
								string choiceText = msg.Substring(q1, q2 - q1);
								int s1 = q2 + 2;
								int s2 = msg.IndexOf(' ', s1+1);

								string choiceType = msg.Substring(s1, s2 - s1);
								if (choiceType == "CARDPILE")
								{
									string[] c = msg.Substring(s2 + 1).Split(' ');
									string choiceSource = c[0];
									int minChoices = int.Parse(c[1]);
									int maxChoices = int.Parse(c[2]);
									int nChoices = int.Parse(c[3]);
									Debug.Assert(nChoices == c.Length - 4);
									Console.WriteLine(choiceText);
									Console.WriteLine("Choose between " + minChoices + " and " + maxChoices);
									for (int i = 4; i < c.Length; i++)
									{
										Console.Write(c[i] + " ");
									}
									Console.WriteLine();
									string input = Console.ReadLine();
									Send("GAME|" + input, s);
								}
								else
								{
									Debug.Assert(choiceType == "EFFECT");
									s1 = s2 + 1;
									s2 = msg.IndexOf(' ', s1);
									int minChoices = int.Parse(msg.Substring(s1, s2 - s1));

									s1 = s2 + 1;
									s2 = msg.IndexOf(' ', s1);
									int maxChoices = int.Parse(msg.Substring(s1, s2 - s1));

									s1 = s2 + 1;
									s2 = msg.IndexOf(' ', s1);
									int numChoices = int.Parse(msg.Substring(s1, s2 - s1));

									Console.WriteLine(choiceText);
									Console.WriteLine("Choose between " + minChoices + " and " + maxChoices);
									for (int i = 0; i < numChoices; i++)
									{
										s1 = s2 + 1;
										s2 = msg.IndexOf(' ', s1 + 1);
										string ci = msg.Substring(s1, s2 - s1);

										q1 = msg.IndexOf('\"', s2) + 1;
										q2 = msg.IndexOf('\"', q1);
										string cdi = msg.Substring(q1, q2 - q1);
										s2 = q2;
										Console.WriteLine(cdi + " " + ci);
									}
									Console.WriteLine();
									string input = Console.ReadLine();
									Send("GAME|" + input, s);
								}
							}
							else if (msg.StartsWith(GameMessages.SupplyPileInfo))
							{
								Console.WriteLine("The available cards are " + msg.Substring(GameMessages.SupplyPileInfo.Length));
						//		synchronizer.HandleClientStateMessage(msg);
							}
							else if (msg.StartsWith(GameMessages.PlayerInfo))
							{
								Console.WriteLine("The players are " + msg.Substring(GameMessages.PlayerInfo.Length));
						//		synchronizer.HandleClientStateMessage(msg);
							}
							else if (msg.StartsWith(GameMessages.PlayerState) || msg.StartsWith(GameMessages.PileState) || msg.StartsWith(GameMessages.CardPileState))
							{
						//		synchronizer.HandleClientStateMessage(msg);
							}
							else if (msg.StartsWith(GameMessages.Log))
							{
								Console.WriteLine(msg.Substring(GameMessages.Log.Length));
							}
						}
						conn = GetNextMessage(s);
					}
				}
			} while (command != "quit");

			Send("SYSTEM|DISCONNECT", s);
			s.Close();
		}

		private static void PrintClientState(GameModel gameModel, string clientName)
		{
			Console.WriteLine("Game state");
			if (!(gameModel.Bane is YoungWitch))
			{
				Console.WriteLine("Bane: " + gameModel.Bane.Name);
			}
			Console.WriteLine("Current Phase: " + gameModel.CurrentPhase);
			Console.WriteLine("Current Player: " + gameModel.Players[gameModel.CurrentPlayerIndex].Name);
			Console.WriteLine("Turn: " + gameModel.TurnCount);
			foreach (Player player in gameModel.Players)
			{
				Console.WriteLine("Player: " + player.Name);
				if (player.Name == clientName)
				{
					Console.Write("Hand:");
					foreach (CardModel card in player.Hand)
					{
						Console.Write(" " + card.Name);
					}
					Console.WriteLine();
				}
				else
				{
					Console.WriteLine("Hand: {0} cards", player.Hand.Count); 
				}
				Console.WriteLine("Actions: " + player.Actions);
				Console.WriteLine("Buys: " + player.Buys);
				Console.WriteLine("Coin: " + player.Coin);
				Console.WriteLine("Potions: " + player.Potions);
				Console.WriteLine("VP Chips: " + player.VPChips);
				Console.WriteLine("Coin Tokens: " + player.CoinTokens);
			}
			Console.WriteLine("Piles");
			foreach (Pile pile in gameModel.SupplyPiles)
			{
				Console.WriteLine("{0}.  Cost {1}{2}.  Count {3}.", pile.Card.Name, pile.Cost, pile.CostsPotion ? "P" : "", pile.Count);
			}
		}
	}
}
