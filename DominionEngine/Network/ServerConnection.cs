using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;
using System.Threading;

namespace Dominion.Network
{
	public interface IServerConnection
	{
		void HandleMessage(NetworkMessage message);
		void Connected();
	}

	public class ServerConnection : IServerConnection
	{
		public event EventHandler SetupComplete;

		private ServerStateSynchronizer synchronizer;
		private GameViewModel gameViewModel;
		private GameLobbyModel gameLobbyModel;
		public PlayerViewModel Player { get; private set; }
		public PlayerViewModel PossessedPlayer { get; private set; }

		private ISocket socket;
		private string username;

		public string Username { get { return this.username; } }

		public bool IsConnected { get { return this.socket.IsConnected; } }

		public ServerConnection(GameLobbyModel gameLobbyModel, ISocket socket)
		{
			this.gameLobbyModel = gameLobbyModel;
			this.gameLobbyModel.HookServerConnection(this);
			
			this.socket = socket;
			this.socket.SetServerConnection(this);
			this.gameStartEvent = new ManualResetEvent(false);
		}

		public void Connect(string address, string username)
		{
			this.socket.Connect(address, username);
		}

		public void Disconnect()
		{
			this.socket.Disconnect();
			this.gameStartEvent.Reset();
		}

		public void SetGameViewModel(GameViewModel gameViewModel)
		{
			this.gameViewModel = gameViewModel;
			this.Player = null;
			this.PossessedPlayer = null;
			this.synchronizer = new ServerStateSynchronizer(gameViewModel.GameModel);
			this.gameStartEvent.Set();
		}

		public void SendSystemMessage(NetworkMessage message)
		{
			this.socket.SendMessage(message);
		}

		public void SendGameMessage(NetworkMessage message)
		{
			this.socket.SendMessage(message);
		}

		private ManualResetEvent gameStartEvent;

		private bool initialized = false;
		private object messageLock = new object();
		public void HandleMessage(NetworkMessage message)
		{
			lock (this.messageLock)
			{
				if (message.MessageCategory == SystemMessages.SystemPrefix)
				{
					switch (message.MessageType)
					{
						case SystemMessages.Authenticate:
						{
							AuthenticationInfo auth = NetworkSerializer.Deserialize<AuthenticationInfo>(message.MessageContent);
							this.username = auth.Username;
						}
							break;
						case SystemMessages.ProposeGame:
						{
							ViewModelDispatcher.BeginInvoke(new Action(delegate()
							{
								GameSpecificationInfo gameSpecification = NetworkSerializer.Deserialize<GameSpecificationInfo>(message.MessageContent);
								this.gameLobbyModel.ServerProposeGame(gameSpecification);
							}));
						}
							break;
						case SystemMessages.CancelRequest:
						{
							ViewModelDispatcher.BeginInvoke(new Action(delegate()
							{
								this.gameLobbyModel.ServerCancelGame();
							}));
						}
							break;
						case SystemMessages.GameStarted:
						{
							this.gameStartEvent.Reset();
							ViewModelDispatcher.BeginInvoke(new Action(delegate()
							{
								this.gameLobbyModel.OnServerGameStarted();
							}));
							this.gameStartEvent.WaitOne();
						}
							break;
						case SystemMessages.SendUsers:
						{
							ViewModelDispatcher.BeginInvoke(new Action(delegate()
							{
								UserInfo userInfo = NetworkSerializer.Deserialize<UserInfo>(message.MessageContent);
								
								this.gameLobbyModel.Users.Clear();

								for (int i = 0; i < userInfo.Users.Count; i++)
								{
									this.gameLobbyModel.Users.Add(userInfo.Users[i]);
								}
							}));
						}
							break;
						case SystemMessages.AddUser:
						{
							ViewModelDispatcher.BeginInvoke(new Action(delegate()
							{
								AddRemoveUserInfo userInfo = NetworkSerializer.Deserialize<AddRemoveUserInfo>(message.MessageContent);
								
								this.gameLobbyModel.Users.Add(userInfo.User);
							}));
						}
							break;
						case SystemMessages.RemoveUser:
						{
							ViewModelDispatcher.BeginInvoke(new Action(delegate()
							{
								AddRemoveUserInfo userInfo = NetworkSerializer.Deserialize<AddRemoveUserInfo>(message.MessageContent);

								this.gameLobbyModel.Users.Remove(userInfo.User);
							}));
						}
							break;
						case SystemMessages.SendLobbies:
						{
							ViewModelDispatcher.BeginInvoke(new Action(delegate()
							{
								LobbyInfo lobbyInfo = NetworkSerializer.Deserialize<LobbyInfo>(message.MessageContent);
								
								this.gameLobbyModel.LobbyName = lobbyInfo.Lobbies[0];
								for (int i = 0; i < lobbyInfo.Lobbies.Count; i++)
								{
									this.gameLobbyModel.Lobbies.Add(lobbyInfo.Lobbies[i]);
								}
								// enter default lobby
								NetworkMessage enterLobbyMessage = new NetworkMessage();
								enterLobbyMessage.MessageCategory = SystemMessages.SystemPrefix;
								enterLobbyMessage.MessageType = SystemMessages.EnterLobby;
								EnterLobbyInfo enterLobby = new EnterLobbyInfo();
								enterLobby.Lobby = lobbyInfo.Lobbies[0];
								enterLobbyMessage.MessageContent = NetworkSerializer.Serialize(enterLobby);
								this.SendSystemMessage(enterLobbyMessage);
							}));
						}
						break;
						case SystemMessages.SendChat:
						{
							ViewModelDispatcher.BeginInvoke(new Action(delegate()
							{
								ChatInfo chatInfo = NetworkSerializer.Deserialize<ChatInfo>(message.MessageContent);
								// does this need to split by '\n'?
								this.gameLobbyModel.Chat.Add(chatInfo.Chat);
							}));
						}
						break;
					}
				}
				else if(message.MessageCategory == GameMessages.GamePrefix)
				{
					switch (message.MessageType)
					{
						case GameMessages.GameEnded:
							{
								ViewModelDispatcher.BeginInvoke(new Action(delegate()
								{
									this.gameViewModel.GameModel.GameOver = true;
								}));
							}
							break;
						case GameMessages.TurnEnded:
							{
								ViewModelDispatcher.BeginInvoke(new Action(delegate()
								{
									this.gameViewModel.FireTurnEnded();
								}));
							}
							break;
						case GameMessages.RequestAction:
							{
								if (!initialized)
								{
									initialized = true;
								}
								PlayerAction action = this.gameViewModel.RequestUIPlayerAction();
								NetworkMessage actionMessage = new NetworkMessage();
								actionMessage.MessageCategory = GameMessages.GamePrefix;
								switch (action.ActionType)
								{
									case ActionType.PlayCard:
										actionMessage.MessageType = GameMessages.PlayCard;
										actionMessage.MessageContent = NetworkSerializer.Serialize(new ActionCardInfo() { Card = action.Card.ID });
										this.SendGameMessage(actionMessage);
										break;
									case ActionType.PlayBasicTreasures:
										actionMessage.MessageType = GameMessages.PlayBasicTreasure;
										this.SendGameMessage(actionMessage);
										break;
									case ActionType.EnterBuyPhase:
										actionMessage.MessageType = GameMessages.BuyPhase;
										this.SendGameMessage(actionMessage);
										break;
									case ActionType.BuyCard:
										actionMessage.MessageType = GameMessages.BuyCard;
										actionMessage.MessageContent = NetworkSerializer.Serialize(new ActionCardInfo() { Card = action.Pile.Card.ID });
										this.SendGameMessage(actionMessage);
										break;
									case ActionType.EndTurn:
										actionMessage.MessageType = GameMessages.EndTurn;
										this.SendGameMessage(actionMessage);
										break;
									case ActionType.CleanupCard:
										actionMessage.MessageType = GameMessages.CleanupCard;
										actionMessage.MessageContent = NetworkSerializer.Serialize(new ActionCardInfo() { Card = action.Card.ID });
										this.SendGameMessage(actionMessage);
										break;
									case ActionType.PlayCoinTokens:
										actionMessage.MessageType = GameMessages.PlayCoinTokens;
										this.SendGameMessage(actionMessage);
										break;
								}
							}
							break;
						case GameMessages.RequestChoice:
							{
								RequestChoiceInfo choiceInfo = NetworkSerializer.Deserialize<RequestChoiceInfo>(message.MessageContent);
								string choiceText = choiceInfo.ChoiceText;

								string choiceType = choiceInfo.ChoiceType;
								bool order = false;
								if (choiceType == "ORDER")
								{
									choiceType = "CARD";
									order = true;
								}
								if (choiceType == "CARD" || choiceType == "PILE")
								{
									PlayerChoiceParameters parameters = new PlayerChoiceParameters();
									parameters.MinChoices = choiceInfo.MinChoices;
									parameters.MaxChoices = choiceInfo.MaxChoices;
									ChoiceSource source = (ChoiceSource)Enum.Parse(typeof(ChoiceSource), choiceInfo.ChoiceSource, true);
									parameters.Source = source;
									parameters.Order = order;
									if (choiceType == "CARD")
									{
										parameters.SourceType = ChoiceSourceType.Card;
										List<CardViewModel> potential = new List<CardViewModel>();
										List<CardViewModel> cardChoices = new List<CardViewModel>();
										if ((source & ChoiceSource.FromHand) != 0)
										{
											potential.AddRange(this.Player.Hand);
										}
										if ((source & ChoiceSource.FromPile) != 0)
										{
											potential.AddRange(this.gameViewModel.Piles.Select(p => p.TopCard));
										}
										if ((source & ChoiceSource.FromTrash) != 0)
										{
											potential.AddRange(this.gameViewModel.Trash);
										}
										if ((source & ChoiceSource.InPlay) != 0)
										{
											potential.AddRange(this.Player.Played);
										}
										foreach(string c in choiceInfo.Choices)
										{
											CardViewModel cvm = potential.FirstOrDefault(card => card.ID == c);
											if (cvm == null && (source & ChoiceSource.None) != 0)
											{
												cvm = new CardViewModel(CardModelFactory.GetCardModel(c));
											}
											potential.Remove(cvm);
											cardChoices.Add(cvm);
										}
										parameters.CardChoices = cardChoices;
									}
									else
									{
										parameters.SourceType = ChoiceSourceType.Pile;
										List<PileViewModel> pileChoices = new List<PileViewModel>();
										foreach (string c in choiceInfo.Choices)
										{
											pileChoices.Add(this.gameViewModel.Piles.First(p => p.Card.ID == c));
										}
										parameters.PileChoices = pileChoices;
									}

									parameters.ChoiceText = choiceText;

									PlayerChoice choice = this.gameViewModel.RequestUIPlayerChoice(parameters);
									NetworkMessage makeChoiceMessage = new NetworkMessage();
									makeChoiceMessage.MessageCategory = GameMessages.GamePrefix;
									makeChoiceMessage.MessageType = GameMessages.MakeChoice;
									ChoiceInfo makeChoiceInfo = new ChoiceInfo();
									
									if (choiceType == "CARD")
									{
										makeChoiceInfo.Choices = choice.ChosenCards.Select(c => c.ID).ToList();
									}
									else
									{
										makeChoiceInfo.Choices = choice.ChosenPiles.Select(p => p.ID).ToList();
									}
									makeChoiceMessage.MessageContent = NetworkSerializer.Serialize(makeChoiceInfo);
									this.SendGameMessage(makeChoiceMessage);
								}
								else
								{
									Debug.Assert(choiceType == "EFFECT");

									PlayerChoiceParameters parameters = new PlayerChoiceParameters();
									parameters.MinChoices = choiceInfo.MinChoices;
									parameters.MaxChoices = choiceInfo.MaxChoices;
									parameters.ChoiceText = choiceText;
									parameters.SourceType = ChoiceSourceType.Effect;
									List<EffectViewModel> effectChoices = new List<EffectViewModel>();
									for (int i = 0; i < choiceInfo.Choices.Count;i++)
									{
										effectChoices.Add(new EffectViewModel(choiceInfo.Choices[i], choiceInfo.ChoiceDescriptions[i]));
									}
									parameters.EffectChoices = effectChoices;
									PlayerChoice playerChoice = this.gameViewModel.RequestUIPlayerChoice(parameters);

									NetworkMessage makeChoiceMessage = new NetworkMessage();
									makeChoiceMessage.MessageCategory = GameMessages.GamePrefix;
									makeChoiceMessage.MessageType = GameMessages.MakeChoice;
									ChoiceInfo makeChoiceInfo = new ChoiceInfo();
									makeChoiceInfo.Choices = playerChoice.ChosenEffects.Select(e => e.Choice).ToList();
									makeChoiceMessage.MessageContent = NetworkSerializer.Serialize(makeChoiceInfo);
									this.SendGameMessage(makeChoiceMessage);
								}
							}
							break;
						case GameMessages.SupplyPileInfo:
						case GameMessages.ExtraPileInfo:
							{
								synchronizer.HandleClientStateMessage(message);
							}
							break;
						case GameMessages.PlayerInfo:
							{
								synchronizer.HandleClientStateMessage(message);
							}
							break;
						case GameMessages.PlayerState:
						case GameMessages.PileState:
						case GameMessages.CardPileState:
							{
								synchronizer.HandleClientStateMessage(message);
							}
							break;
						case GameMessages.GameState:
							{
								synchronizer.HandleClientStateMessage(message);
							}
							break;
						case GameMessages.Log:
							{
								ViewModelDispatcher.BeginInvoke(new Action(delegate()
								{
									LogInfo logInfo = NetworkSerializer.Deserialize<LogInfo>(message.MessageContent);
									this.gameViewModel.GameModel.TextLog.WriteLine(logInfo.Log);
								}));
							}
							break;
						case GameMessages.StartTurn:
							{
								ViewModelDispatcher.BeginInvoke(new Action(delegate()
								{
									this.gameViewModel.GameModel.StartTurn();
								}));
							}
							break;
						case GameMessages.SetupComplete:
							{
								ViewModelDispatcher.BeginInvoke(new Action(delegate()
								{
									this.gameViewModel.OnInitialized();
									this.gameViewModel.GameInitialized += gameViewModel_GameInitialized;
								}));
							}
							break;
					}
				}
			}
		}

		private void gameViewModel_GameInitialized(object sender, EventArgs e)
		{
			this.gameViewModel.GameInitialized -= gameViewModel_GameInitialized;
			this.Player = this.gameViewModel.Players.First(p => p.Name == this.username);
			this.PossessedPlayer = this.gameViewModel.GetPlayer(this.gameViewModel.GameModel.PlayerLeftOf(this.Player.PlayerModel));
			if (this.SetupComplete != null)
			{
				this.SetupComplete(this, EventArgs.Empty);
			}
		}

		public void Connected()
		{
			ViewModelDispatcher.BeginInvoke(new Action(() =>
			{
				this.gameLobbyModel.IsVisible = true;
			}));
		}

		public void SendChat(string chat)
		{
			ChatInfo chatInfo = new ChatInfo();
			chatInfo.Chat = chat;
			chatInfo.Name = this.username;
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.SendChat;
			message.MessageContent = NetworkSerializer.Serialize(chatInfo);
			this.SendSystemMessage(message);
		}

		public void Close()
		{
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.Disconnect;
			this.SendSystemMessage(message);
		}
	}

}
