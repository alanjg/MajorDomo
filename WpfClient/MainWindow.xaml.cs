using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dominion;
using System.Net.Sockets;
using DominionServer;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Dominion.Network;

namespace WpfClient
{
	public enum ClientState
	{
		// connection states
		None,
		Connecting,
		Connected,
		Lobby,
		NegotiatingGame,

		// game states
		GameStarted,
		OpponentActing,
		Waiting,
		PlayerActing,
		PlayerReacting,
		PlayerChoosing,
		GameOver
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//private PlayerState localPlayer;
		private GameModel viewModel;
		private GameModel localGameModel;
		Socket socket = null;
		ServerStateSynchronizer synchronizer;
		private ClientState clientState = ClientState.None;
		private bool HandleMessage(string message)
		{
			if (message.StartsWith("AUTHENTICATE"))
			{
				int start = SystemMessages.Authenticate.Length + 1;
				int end = message.IndexOf('\"', start);
				string username = message.Substring(start, end - start);
				string auth = message.Substring(end + 2);
			}
			else if (message.StartsWith("SENDUSERS"))
			{
				string[] users = message.Split(' ');

				this.Users.Items.Clear();

				for (int i = 1; i < users.Length; i++)
				{
					if (users[i] != this.playerName)
					{
						this.Users.Items.Add(users[i]);
					}
				}
			}
			else if (message.StartsWith("NOCHAT"))
			{

			}
			else if (message.StartsWith("GETCHAT"))
			{
				string[] chat = message.Substring(8).Split('\n');
				foreach (string c in chat)
				{
					this.ChatText.Inlines.Add(new Run() { Text = c });
					this.ChatText.Inlines.Add(new LineBreak());
				}
			}
			else if (message == GameMessages.GamePrefix + GameMessages.GameEnded)
			{
				// end game
				return true;
			}
			else
			{
				if (message.StartsWith("GAME|"))
				{
					string msg = message.Substring(5);
					if (msg == GameMessages.RequestAction)
					{
						this.clientState = ClientState.PlayerActing;
						//this.SendMessage("GAME|" + req);
					}
					else if (msg.StartsWith(GameMessages.RequestChoice))
					{
						this.clientState = ClientState.PlayerChoosing;
						int q1 = msg.IndexOf('\"') + 1;
						int q2 = msg.IndexOf('\"', q1);
						string choiceText = msg.Substring(q1, q2 - q1);
						int s1 = q2 + 2;
						int s2 = msg.IndexOf(' ', s1 + 1);

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
							//string input = Console.ReadLine();
							//Send("GAME|" + input, s);
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
							//Send("GAME|" + input, s);
						}
					}
					else if (msg.StartsWith(GameMessages.SupplyPileInfo))
					{
						Console.WriteLine("The available cards are " + msg.Substring(GameMessages.SupplyPileInfo.Length));
			//			synchronizer.HandleClientStateMessage(msg);
					}
					else if (msg.StartsWith(GameMessages.PlayerInfo))
					{
						Console.WriteLine("The players are " + msg.Substring(GameMessages.PlayerInfo.Length));
			//			synchronizer.HandleClientStateMessage(msg);
						//this.DataContext = new GameViewModel() { Player = this.viewModel.Players.First(p => p.Name == this.playerName), GameModel = this.viewModel };
					}
					else if (msg.StartsWith(GameMessages.PlayerState) || msg.StartsWith(GameMessages.PileState) || msg.StartsWith(GameMessages.CardPileState))
					{
			//			synchronizer.HandleClientStateMessage(msg);
					}
					else if (msg.StartsWith(GameMessages.Log))
					{
						Console.WriteLine(msg.Substring(GameMessages.Log.Length));
					}
				}
			}
			return false;
		}

		private void SendChat(string chat)
		{
			this.Send("SYSTEM|SENDCHAT " + chat);
		}

		DateTime lastChat = DateTime.Now;
		private void GetChat()
		{
			this.Send("SYSTEM|GETCHAT " + lastChat.ToString());
			this.lastChat = DateTime.Now;
		}

		private void GetUsers()
		{
			Send("SYSTEM|GETUSERS");
		}

		//private bool serverGame = false;
		private string serverAddress;
		private string playerName;
		private string serverAuthentication;
		public MainWindow()
		{
			InitializeComponent();

			this.viewModel = new GameModel();
			//this.DataContext = this.viewModel;
			this.synchronizer = new ServerStateSynchronizer(this.viewModel);
			this.PlayArea.Visibility = Visibility.Hidden;
			this.ServerLobby.Visibility = Visibility.Hidden;
			this.GameSelect.Visibility = Visibility.Visible;

			this.AddressText.Text = Dns.GetHostName();
		}
		
		private void connectClicked(object sender, System.Windows.RoutedEventArgs e)
		{
			this.playerName = this.IDText.Text;
			if (this.ServerRadio.IsChecked.Value)
			{
				//this.serverGame = true;
				this.serverAddress = this.AddressText.Text;
				int port = 4502;
				this.socket = ConnectSocket(this.serverAddress, port);

				this.Send("CONNECT " + this.playerName);
				string result = GetNextMessage();
				Console.WriteLine(result);
				this.serverAuthentication = result.Substring(13);
				Thread thread = new Thread(new ThreadStart(socketListener));
				thread.Start();
				this.PlayArea.Visibility = Visibility.Hidden;
				this.ServerLobby.Visibility = Visibility.Visible;
				this.GameSelect.Visibility = Visibility.Hidden;
				Timer t = new Timer(new TimerCallback(o =>
				{
					this.GetChat();
					this.GetUsers();
				}), null, 0, 1000);
			}
			else
			{
				this.localGameModel = new GameModel();
			}			
		}

		private void socketListener()
		{
			do
			{
				string message = this.GetNextMessage();
				this.Dispatcher.BeginInvoke(new DispatcherOperationCallback(this.socketListenerDispatcher), message);
				
			} while(true);
		}

		private object socketListenerDispatcher(object arg)
		{
			this.HandleMessage((string)arg);
			return null;
		}


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

				try
				{

					tempSocket.Connect(ipe);
				}
				catch
				{
					continue;
				}
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


		private string Receive()
		{
			Byte[] bytesReceived = new Byte[256];
			int bytes = 0;
			string result = string.Empty;

			do
			{
				bytes = this.socket.Receive(bytesReceived, bytesReceived.Length, 0);
				result = result + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
			}
			while (bytes == bytesReceived.Length);
			return result;
		}
		private Queue<string> messageQueue = new Queue<string>();
		private string GetNextMessage()
		{
			if (messageQueue.Count > 0)
			{
				return messageQueue.Dequeue();
			}
			string[] messages = this.Receive().Split('$');
			foreach (string msg in messages)
			{
				messageQueue.Enqueue(msg);
			}
			return messageQueue.Dequeue();
		}

		private void Send(string message)
		{
			Byte[] bytesSent = Encoding.ASCII.GetBytes(message);
			this.socket.Send(bytesSent, bytesSent.Length, 0);
		}

		private void RequestGame(object sender, System.Windows.RoutedEventArgs e)
		{
			if (this.Users.SelectedItem != null)
			{
				string user = (string)this.Users.SelectedItem.ToString();
				Send("SYSTEM|REQUESTGAME " + user);
			}
		}

		private void AcceptGame(object sender, System.Windows.RoutedEventArgs e)
		{
			this.clientState = ClientState.GameStarted;
			this.PlayArea.Visibility = Visibility.Visible;
			this.ServerLobby.Visibility = Visibility.Hidden;
			this.GameSelect.Visibility = Visibility.Hidden;
		}

		private void sendChat(object sender, System.Windows.RoutedEventArgs e)
		{
			this.SendChat(this.InputChat.Text);
		}

		private void cardMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			FrameworkElement source = (FrameworkElement)sender;
			CardModel card = (CardModel)source.DataContext;

			if (this.clientState == ClientState.PlayerActing)
			{
				if (this.clientState == ClientState.PlayerActing && (this.viewModel.Players[this.viewModel.CurrentPlayerIndex].Actions > 0 && this.viewModel.CurrentPhase == GamePhase.Action || card.Is(CardType.Treasure) && this.viewModel.CurrentPhase == GamePhase.Buy))
				{
					this.clientState = ClientState.Waiting;
					Send("GAME|PLAYCARD " + card.ID);
				}
			}
			else if (this.clientState == ClientState.PlayerChoosing)
			{

			}
		}

		private void pileMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			FrameworkElement source = (FrameworkElement)sender;
			Pile pile = (Pile)source.DataContext;

			if (this.clientState == ClientState.PlayerActing)
			{
				if (this.clientState == ClientState.PlayerActing && this.viewModel.CurrentPhase == GamePhase.Buy)
				{
					this.clientState = ClientState.Waiting;
					Send("GAME|BUYCARD " + pile.Card.ID);
				}
			}
			else if (this.clientState == ClientState.PlayerChoosing)
			{

			}
		}

		private void playAllCoinClicked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (this.clientState == ClientState.PlayerActing)
			{
				this.clientState = ClientState.Waiting;
				Send("GAME|PLAYBASICTREASURE");	
			}
		}

		private void buyPhaseClicked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (this.clientState == ClientState.PlayerActing)
			{
				if (this.viewModel.CurrentPhase == GamePhase.Action)
				{
					this.clientState = ClientState.Waiting;
					Send("GAME|BUYPHASE");
				}
			}
			
		}

		private void endTurnClicked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (this.clientState == ClientState.PlayerActing)
			{
				this.clientState = ClientState.Waiting;
				Send("GAME|ENDTURN");
			}
		}
		
		/*
		public void OnExit()
		{
			if (this.serverGame)
			{
				this.SendMessage("SYSTEM|DISCONNECT");
			}
		}
		 * */
	}

}
