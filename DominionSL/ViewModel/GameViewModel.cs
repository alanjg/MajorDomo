using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dominion;
using Dominion.Model.Actions;

namespace DominionSL
{
	public enum ClientState
	{
		NotPlaying,
		WaitingForOpponent,
		WaitingForPlayerAction,
		WaitingForPlayerChoice,
		WaitingForPlayerReaction
	}

	public class GameViewModel : INotifyPropertyChanged
	{
		private ClientState state = ClientState.NotPlaying;
		private GameModel gameModel;
		private List<PlayerViewModel> players = new List<PlayerViewModel>();
		private ObservableCollection<PileViewModel> piles = new ObservableCollection<PileViewModel>();
		private List<CardViewModel> prizes = new List<CardViewModel>();

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

		//public event EventHandler<ItemBoughtEventArgs> ItemBought;
		public event EventHandler GameInitialized;
		private PlayerAction uiRequestAction;
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
			this.uiPlayerActionWaitHandle.WaitOne();
			return this.uiRequestAction;
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
			get { return this.gameModel.CanBuy && this.CurrentPlayer.Coin > 0; }
		}

		public bool CanPlayAction
		{
			get { return this.gameModel.CanPlayAction && this.CurrentPlayer.Actions > 0 && this.CurrentPlayer.HasActions; }
		}

		public bool CanPlayTreasure
		{
			get { return this.gameModel.CanPlayTreasure && this.CurrentPlayer.Hand.Any(card => card.Is(CardType.Treasure)); }
		}

		public bool GameHasPotions
		{
			get { return this.piles.Any(pile => pile.Card.CardModel is Potion); }
		}

		public bool GameHasPirateShip
		{
			get { return this.piles.Any(pile => pile.Card.CardModel is PirateShip); }
		}

		public bool GameHasNativeVillage
		{
			get { return this.piles.Any(pile => pile.Card.CardModel is NativeVillage); }
		}

		public bool GameHasVPChips
		{
			get { return this.piles.Any(pile => pile.Card.CardModel is Goons || pile.Card.CardModel is Bishop || pile.Card.CardModel is Monument); }
		}

		public PlayerViewModel CurrentPlayer
		{
			get { return this.GetPlayer(this.gameModel.CurrentPlayer); }
		}

		public List<PlayerViewModel> Players
		{
			get { return this.players; }
		}

		public ObservableCollection<PileViewModel> Piles
		{
			get { return this.piles; }
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

		private LogViewModel log;
		public GameViewModel(GameModel gameModel)
		{
			this.gameModel = gameModel;
			this.log = new LogViewModel(this.gameModel.TextLog);
			this.gameModel.ModelUpdated += new EventHandler(gameModel_ModelUpdated);
			this.gameModel.RefreshUI += new EventHandler(gameModel_RefreshUI);
			//	this.gameModel.ItemBought += new EventHandler<Dominion.GameModel.ItemBoughtEventArgs>(gameModel_ItemBought);
			this.gameModel.GameInitialized += new EventHandler(gameModel_GameInitialized);
			this.gameModel.PropertyChanged += new PropertyChangedEventHandler(gameModel_PropertyChanged);
		}

		void gameModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged(e.PropertyName);
			}));
		}

		void gameModel_GameInitialized(object sender, EventArgs e)
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				foreach (Player player in this.gameModel.Players)
				{
					this.players.Add(new PlayerViewModel(player));
				}

				foreach (Pile pile in this.gameModel.Piles)
				{
					this.piles.Add(new PileViewModel(pile, this));
				}

				foreach (CardModel prize in this.gameModel.Prizes)
				{
					this.prizes.Add(new CardViewModel(prize));
				}
				this.OnPropertyChanged("Players");
				this.OnPropertyChanged("CurrentPlayer");
				this.OnPropertyChanged("Prizes");

				if (this.GameInitialized != null)
				{
					this.GameInitialized(this, EventArgs.Empty);
				}
			}));
		}
		/*
		private void gameModel_ItemBought(object sender, GameModel.ItemBoughtEventArgs e)
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				if (this.ItemBought != null)
				{
					this.ItemBought(this, new ItemBoughtEventArgs(new CardViewModel(e.PurchasedCard), this.GetPlayer(e.PurchasingPlayer)));
				}
			}));
		}
		*/
		public PlayerViewModel GetPlayer(Player player)
		{
			return this.players.FirstOrDefault(p => p.PlayerModel == player);
		}

		void gameModel_RefreshUI(object sender, EventArgs e)
		{
			this.RefreshUI();
		}

		private void gameModel_ModelUpdated(object sender, EventArgs e)
		{
			this.UpdateModel();
		}

		private void UpdateModel()
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				foreach (PileViewModel pile in this.Piles)
				{
					pile.Update();
				}
				this.OnPropertyChanged("CurrentPhase");
				this.OnPropertyChanged("CanPlayTreasure");
			}));
		}

		// todo jekelly: clean this up
		public void RefreshUI()
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged("CanPlayAction");
				this.OnPropertyChanged("CanBuy");
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