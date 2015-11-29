using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Dominion;
using Dominion.Model.Actions;
using DominionEngine.AI;

namespace Dominion
{
	public enum ClientState
	{
		NotPlaying,
		WaitingForOpponent,
		WaitingForPlayerAction,
		WaitingForPlayerChoice
	}

	public delegate void RequestPlayerActionHandler(GameViewModel sender);
	public delegate void RequestPlayerChoiceHandler(GameViewModel sender, PlayerChoiceParameters parameters);

	public class GameViewModel : INotifyPropertyChanged
	{
		private ClientState state = ClientState.NotPlaying;
		private GameModel gameModel;
		private ObservableCollection<PlayerViewModel> players = new ObservableCollection<PlayerViewModel>();
		private ObservableCollection<PileViewModel> piles = new ObservableCollection<PileViewModel>();
		private ObservableCollection<PileViewModel> basicTreasurePiles = new ObservableCollection<PileViewModel>();
		private ObservableCollection<PileViewModel> basicVictoryPiles = new ObservableCollection<PileViewModel>();
		private ObservableCollection<PileViewModel> kingdomPiles = new ObservableCollection<PileViewModel>();
		private ObservableCollection<PileViewModel> kingdomPiles1 = new ObservableCollection<PileViewModel>();
		private ObservableCollection<PileViewModel> kingdomPiles2 = new ObservableCollection<PileViewModel>();
		private ObservableCollection<PileViewModel> kingdomPiles1of3 = new ObservableCollection<PileViewModel>();
		private ObservableCollection<PileViewModel> kingdomPiles2of3 = new ObservableCollection<PileViewModel>();
		private ObservableCollection<PileViewModel> kingdomPiles3of3 = new ObservableCollection<PileViewModel>();
		private ObservableCollection<CardViewModel> prizes = new ObservableCollection<CardViewModel>();
		private ObservableCollection<CardViewModel> trash = new ObservableCollection<CardViewModel>();

		public GameModel GameModel { get { return this.gameModel; } }
		public sealed class ItemBoughtEventArgs : EventArgs
		{
			public CardViewModel PurchasedCard
			{
				get;
				private set;
			}

			public PlayerViewModel PurchasingPlayer
			{
				get;
				private set;
			}

			public ItemBoughtEventArgs(CardViewModel purchasedCardID, PlayerViewModel purchasingPlayer)
			{
				this.PurchasedCard = purchasedCardID;
				this.PurchasingPlayer = purchasingPlayer;
			}
		}

		public event EventHandler GameInitialized;
		public event EventHandler OnGameOver;
		public event EventHandler TurnEnded;

		public event RequestPlayerActionHandler RequestPlayerAction;
		public event RequestPlayerChoiceHandler RequestPlayerChoice;

		private PlayerAction uiRequestAction;
		private PlayerChoice uiRequestChoice;

		public PlayerChoiceParameters UIChoiceParameters { get; private set; }
		private ManualResetEvent uiPlayerActionWaitHandle = new ManualResetEvent(true);
		public void PerformUIPlayerAction(ActionType actionType, CardViewModel card, PileViewModel pile)
		{
			this.uiRequestAction = new PlayerAction();
			this.uiRequestAction.ActionType = actionType;
			this.uiRequestAction.Card = card != null ? card.CardModel : null;
			this.uiRequestAction.Pile = pile != null ? pile.PileModel : null;
			this.state = ClientState.WaitingForOpponent;
			this.uiPlayerActionWaitHandle.Set();
		}

		public PlayerAction RequestUIPlayerAction()
		{
			this.state = ClientState.WaitingForPlayerAction;
			this.uiPlayerActionWaitHandle.Reset();
			ViewModelDispatcher.BeginInvoke(new Action(() => 
			{
				if(this.RequestPlayerAction != null)
				{
					this.RequestPlayerAction(this);
				}
			}));
			this.uiPlayerActionWaitHandle.WaitOne();
			return this.uiRequestAction;
		}

		public void PerformUIPlayerChoice(IEnumerable<CardViewModel> chosenCards, IEnumerable<PileViewModel> chosenPiles, IEnumerable<EffectViewModel> chosenEffects)
		{
			this.uiRequestChoice = new PlayerChoice();
			this.uiRequestChoice.ChosenCards = chosenCards;
			this.uiRequestChoice.ChosenEffects = chosenEffects;
			this.uiRequestChoice.ChosenPiles = chosenPiles;
			this.state = ClientState.WaitingForOpponent;
			this.uiPlayerActionWaitHandle.Set();
		}

		public PlayerChoice RequestUIPlayerChoice(PlayerChoiceParameters parameters)
		{
			this.UIChoiceParameters = parameters;
			this.state = ClientState.WaitingForPlayerChoice;
			this.uiPlayerActionWaitHandle.Reset();
			ViewModelDispatcher.BeginInvoke(new Action(() =>
			{
				if (this.RequestPlayerChoice != null)
				{
					this.RequestPlayerChoice(this, parameters);
				}
			}));
			this.uiPlayerActionWaitHandle.WaitOne();
			return this.uiRequestChoice;
		}

		public void FireTurnEnded()
		{
			if (this.TurnEnded != null)
			{
				this.TurnEnded(this, EventArgs.Empty);
			}
		}

		public ClientState CurrentPlayerState
		{
			get { return this.state; }
		}

		public GamePhase CurrentPhase
		{
			get { return this.gameModel.CurrentPhase; }
		}

		public bool GameStarted
		{
			get { return this.gameModel.GameStarted; }
		}

		public bool GameOver
		{
			get { return this.gameModel.GameOver; }
		}

		public bool CanBuy
		{
			get { return this.gameModel.CanBuy && this.CurrentPlayer != null && this.CurrentPlayer.Coin > 0; }
		}

		public bool CanPlayAction
		{
			get { return this.gameModel.CanPlayAction && this.CurrentPlayer != null && this.CurrentPlayer.Actions > 0; }
		}

		public bool CanPlayTreasure
		{
			get { return this.gameModel.CanPlayTreasure && this.CurrentPlayer != null; }
		}

		public bool GameHasPotions
		{
			get { return this.gameModel.GameHasPotions; }
		}

		public bool GameHasPirateShip
		{
			get { return this.gameModel.GameHasPirateShip; }
		}

		public bool GameHasNativeVillage
		{
			get { return this.gameModel.GameHasNativeVillage; }
		}

		public bool GameHasIsland
		{
			get { return this.gameModel.GameHasIsland; }
		}

		public bool GameHasVPChips
		{
			get { return this.gameModel.GameHasVPChips; }
		}

		public bool GameHasCoinTokens
		{
			get { return this.gameModel.GameHasCoinTokens; }
		}

		public PlayerViewModel CurrentPlayer
		{
			get { return this.GetPlayer(this.gameModel.CurrentPlayer); }
		}

		public ObservableCollection<PlayerViewModel> Players
		{
			get { return this.players; }
		}

		public ObservableCollection<PileViewModel> Piles
		{
			get { return this.piles; }
		}

		public ObservableCollection<PileViewModel> BasicTreasurePiles
		{
			get { return this.basicTreasurePiles; }
		}

		public ObservableCollection<PileViewModel> BasicVictoryPiles
		{
			get { return this.basicVictoryPiles; }
		}

		public ObservableCollection<PileViewModel> KingdomPiles
		{
			get { return this.kingdomPiles; }
		}

		public ObservableCollection<PileViewModel> KingdomPiles1
		{
			get { return this.kingdomPiles1; }
		}

		public ObservableCollection<PileViewModel> KingdomPiles2
		{
			get { return this.kingdomPiles2; }
		}

		public ObservableCollection<PileViewModel> KingdomPiles1of3
		{
			get { return this.kingdomPiles1of3; }
		}

		public ObservableCollection<PileViewModel> KingdomPiles2of3
		{
			get { return this.kingdomPiles2of3; }
		}

		public ObservableCollection<PileViewModel> KingdomPiles3of3
		{
			get { return this.kingdomPiles3of3; }
		}

		public ObservableCollection<CardViewModel> Trash
		{
			get { return this.trash; }
		}

		public CardModel Bane
		{
			get { return this.gameModel.Bane; }
		}

		public int TradeRouteCount
		{
			get { return this.gameModel.TradeRouteCount; }
		}

		public IList<CardModifier> CardModifiers
		{
			get { return this.gameModel.CardModifiers; }
		}

		public LogViewModel TextLog
		{
			get
			{
				return this.log;
			}
		}

		public bool ShowPlayerScoreInDetails
		{
			get { return this.gameModel.ShowPlayerScoreInDetails; }
		}
		

		private LogViewModel log;
		public GameViewModel(GameModel gameModel)
		{
			this.gameModel = gameModel;
			this.log = new LogViewModel(this.gameModel.TextLog);
			this.gameModel.PileBuyStatusChanged += new EventHandler(gameModel_PileBuyStatusChanged);
			this.gameModel.PlayerActionsAndPhaseUpdated += new EventHandler(gameModel_PlayerActionsAndPhaseUpdated);
			this.gameModel.GameInitialized += new EventHandler(gameModel_GameInitialized);
			this.gameModel.PropertyChanged += new PropertyChangedEventHandler(gameModel_PropertyChanged);
			this.gameModel.OnGameOver += gameModel_OnGameOver;
		}

		private void gameModel_OnGameOver(object sender, EventArgs e)
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				if (this.OnGameOver != null)
				{
					this.OnGameOver(this, EventArgs.Empty);
				}
			}));
		}

		private void gameModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged(e.PropertyName);
			}));
		}

		private void gameModel_GameInitialized(object sender, EventArgs e)
		{
			this.OnInitialized();
		}

		public void OnInitialized()
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				foreach (Player player in this.gameModel.Players)
				{
					this.players.Add(new PlayerViewModel(player, this));
				}

				foreach (Pile pile in this.gameModel.SupplyPiles)
				{
					PileViewModel pvm = new PileViewModel(pile, this);
					this.piles.Add(pvm);
					if (pile.Card is Copper || pile.Card is Silver || pile.Card is Gold || pile.Card is Potion || pile.Card is Platinum)
					{
						this.basicTreasurePiles.Add(pvm);
					}
					else if (pile.Card is Curse || pile.Card is Estate || pile.Card is Duchy || pile.Card is Province || pile.Card is Colony)
					{
						this.basicVictoryPiles.Add(pvm);
					}
					else
					{
						this.kingdomPiles.Add(pvm);
					}
				}

				foreach (Pile pile in this.gameModel.ExtraPiles)
				{
					PileViewModel pvm = new PileViewModel(pile, this);
					this.piles.Add(pvm);
					this.kingdomPiles.Add(pvm);
				}

				foreach (PileViewModel pile in this.kingdomPiles)
				{
					if (this.kingdomPiles1.Count < 10)
					{
						this.kingdomPiles1.Add(pile);
					}
					else
					{
						this.kingdomPiles2.Add(pile);
					}

					if (this.kingdomPiles1of3.Count < 5)
					{
						this.kingdomPiles1of3.Add(pile);
					}
					else if(this.kingdomPiles2of3.Count < 5)
					{
						this.kingdomPiles2of3.Add(pile);
					}
					else
					{
						this.kingdomPiles3of3.Add(pile);
					}
				}
				foreach (CardModel prize in this.gameModel.Prizes)
				{
					this.prizes.Add(new CardViewModel(prize));
				}
				this.OnPropertyChanged("Players");
				this.OnPropertyChanged("CurrentPlayer");
				this.OnPropertyChanged("Prizes");
				new CollectionSynchronizer(this.gameModel.Trash, this.trash);

				if (this.GameInitialized != null)
				{
					this.GameInitialized(this, EventArgs.Empty);
				}
			}));
		}

		public PlayerViewModel GetPlayer(Player player)
		{
			return this.players.FirstOrDefault(p => p.PlayerModel == player);
		}

		private void gameModel_PlayerActionsAndPhaseUpdated(object sender, EventArgs e)
		{
			this.UpdatePlayerActionsAndPhase();
		}

		private void gameModel_PileBuyStatusChanged(object sender, EventArgs e)
		{
			this.UpdatePileBuyStatus();
		}

		public void UpdatePlayerActionsAndPhase()
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged("CanPlayAction");
				this.OnPropertyChanged("CanBuy");
				this.OnPropertyChanged("CurrentPhase");
				this.OnPropertyChanged("CanPlayTreasure");
			}));
		}

		public void UpdatePileBuyStatus()
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				foreach (PileViewModel pile in this.Piles)
				{
					pile.Update();
				}
			}));
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}