using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
	public class PlayerViewModel : INotifyPropertyChanged
	{
		private Player player;
		public Player PlayerModel { get { return this.player; } }

		private ObservableCollection<CardViewModel> hand;
		private ObservableCollection<CardViewModel> discard;
		private ObservableCollection<CardViewModel> cleanup;
		private ObservableCollection<CardViewModel> played;
		private ObservableCollection<CardViewModel> duration;
		private ObservableCollection<CardViewModel> bought;
		private ObservableCollection<CardViewModel> gainedLastTurn;
		private ObservableCollection<CardViewModel> nativeVillageCards;
		private ObservableCollection<CardViewModel> islands;

		public ReadOnlyObservableCollection<CardViewModel> Hand
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> Discard
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> Cleanup
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> Played
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> Duration
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> Bought
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> GainedLastTurn
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> NativeVillageCards
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> Islands
		{
			get;
			private set;
		}

		public DeckViewModel Deck
		{
			get;
			private set;
		}

		public string Name
		{
			get { return this.player.Name; }
		}

		public bool HasActions
		{
			get
			{
				return (from card in this.Hand where card.CardModel.Is(CardType.Action) select card).Any();
			}
		}

		public int Coin
		{
			get
			{
				return this.player.Coin;
			}
		}

		public int Potions
		{
			get
			{
				return this.player.Potions;
			}
		}

		public int Buys
		{
			get
			{
				return this.player.Buys;
			}
		}

		public int Actions
		{
			get
			{
				return this.player.Actions;
			}
		}

		public int VPChips
		{
			get { return this.player.VPChips; }
		}

		public int PirateShipTreasureTokens
		{
			get { return this.player.PirateShipTokens; }
		}

		public PlayerViewModel(Player player)
		{
			this.player = player;
			this.player.Refresh += new EventHandler(player_Refresh);

			((INotifyCollectionChanged)this.player.Hand).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.Hand, this.hand); });
			((INotifyCollectionChanged)this.player.Discard).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.Discard, this.discard); });
			((INotifyCollectionChanged)this.player.Cleanup).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.Cleanup, this.cleanup); });
			((INotifyCollectionChanged)this.player.Played).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.Played, this.played); });
			((INotifyCollectionChanged)this.player.Duration).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.Duration, this.duration); });
			((INotifyCollectionChanged)this.player.Bought).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.Bought, this.bought); });
			((INotifyCollectionChanged)this.player.GainedLastTurn).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.GainedLastTurn, this.gainedLastTurn); });
			((INotifyCollectionChanged)this.player.NativeVillageMat).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.NativeVillageMat, this.nativeVillageCards); });
			((INotifyCollectionChanged)this.player.IslandMat).CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object o, NotifyCollectionChangedEventArgs e) { NotifyChange(e, this.player.IslandMat, this.islands); });
			this.player.PropertyChanged += new PropertyChangedEventHandler(player_PropertyChanged);
			this.hand = new ObservableCollection<CardViewModel>();
			this.Hand = new ReadOnlyObservableCollection<CardViewModel>(this.hand);
			this.discard = new ObservableCollection<CardViewModel>();
			this.Discard = new ReadOnlyObservableCollection<CardViewModel>(this.discard);
			this.cleanup = new ObservableCollection<CardViewModel>();
			this.Cleanup = new ReadOnlyObservableCollection<CardViewModel>(this.cleanup);
			this.played = new ObservableCollection<CardViewModel>();
			this.Played = new ReadOnlyObservableCollection<CardViewModel>(this.played);
			this.duration = new ObservableCollection<CardViewModel>();
			this.Duration = new ReadOnlyObservableCollection<CardViewModel>(this.duration);
			this.bought = new ObservableCollection<CardViewModel>();
			this.Bought = new ReadOnlyObservableCollection<CardViewModel>(this.bought);
			this.gainedLastTurn = new ObservableCollection<CardViewModel>();
			this.GainedLastTurn = new ReadOnlyObservableCollection<CardViewModel>(this.gainedLastTurn);
			this.nativeVillageCards = new ObservableCollection<CardViewModel>();
			this.NativeVillageCards = new ReadOnlyObservableCollection<CardViewModel>(this.nativeVillageCards);
			this.islands = new ObservableCollection<CardViewModel>();
			this.Islands = new ReadOnlyObservableCollection<CardViewModel>(this.islands);

			this.Deck = new DeckViewModel(this.player.Deck);
			foreach (CardModel card in this.player.Hand)
			{
				this.hand.Add(new CardViewModel(card));
			}
		}

		void player_Refresh(object sender, EventArgs e)
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged("Actions");
				this.OnPropertyChanged("Buys");
				this.OnPropertyChanged("Coin");
				this.OnPropertyChanged("Potions");
				this.OnPropertyChanged("VPChips");
			}));
		}

		private void player_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);
		}

		private void NotifyChange(NotifyCollectionChangedEventArgs e, ReadOnlyObservableCollection<CardModel> sourceCollection, ObservableCollection<CardViewModel> targetCollection)
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				//if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					targetCollection.Clear();
					foreach (CardModel card in sourceCollection)
					{
						targetCollection.Add(new CardViewModel(card));
					}
				}

				//	else
				//	{
				//		if (e.OldItems != null)
				//		{
				//			foreach (CardModel card in e.OldItems)
				//			{
				//				targetCollection.Remove(targetCollection.FirstOrDefault(item => item.CardModel == card));
				//			}
				//		}
				//		if (e.NewItems != null)
				//		{
				//			foreach (CardModel card in e.NewItems)
				//			{
				//				targetCollection.Add(new CardViewModel(card));
				//			}
				//		}
				//	}
				Debug.Assert(targetCollection.Count == sourceCollection.Count, "Counts should match");
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