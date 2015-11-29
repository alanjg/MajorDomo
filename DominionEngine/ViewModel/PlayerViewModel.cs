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
using Dominion.Model.Chooser;

namespace Dominion
{
	public class PlayerViewModel : INotifyPropertyChanged
	{
		private GameViewModel gameViewModel;
		public GameViewModel GameViewModel { get { return this.gameViewModel; } }
		private Player player;
		public Player PlayerModel { get { return this.player; } }

		private ObservableCollection<CardViewModel> hand;
		private ObservableCollection<CardViewModel> discard;
		private ObservableCollection<CardViewModel> cleanup;
		private ObservableCollection<CardViewModel> played;
		private ObservableCollection<CardViewModel> duration;
		private ObservableCollection<CardViewModel> bought;
		private ObservableCollection<CardViewModel> gainedLastTurn;
		private ObservableCollection<CardViewModel> nativeVillage;
		private ObservableCollection<CardViewModel> island;
		private ObservableCollection<CardViewModel> tavern;

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

		public ReadOnlyObservableCollection<CardViewModel> NativeVillage
		{
			get;
			private set;
		}

		public ReadOnlyObservableCollection<CardViewModel> Island
		{
			get;
			private set;
		}
		public ReadOnlyObservableCollection<CardViewModel> Tavern
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

		public int CoinTokens
		{
			get { return this.player.CoinTokens; }
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

		public int Points
		{
			get { return this.player.Points; }
		}

		public int PirateShipTreasureTokens
		{
			get { return this.player.PirateShipTokens; }
		}

		public PlayerViewModel(Player player, GameViewModel gameViewModel)
		{
			this.gameViewModel = gameViewModel;
			this.player = player;

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
			this.nativeVillage = new ObservableCollection<CardViewModel>();
			this.NativeVillage = new ReadOnlyObservableCollection<CardViewModel>(this.nativeVillage);
			this.island = new ObservableCollection<CardViewModel>();
			this.Island = new ReadOnlyObservableCollection<CardViewModel>(this.island);
			this.tavern = new ObservableCollection<CardViewModel>();
			this.Tavern = new ReadOnlyObservableCollection<CardViewModel>(this.tavern);

			new CollectionSynchronizer(this.player.Hand, this.hand, true);
			new CollectionSynchronizer(this.player.Discard, this.discard, true);
			new CollectionSynchronizer(this.player.Cleanup, this.cleanup, true);
			new CollectionSynchronizer(this.player.Played, this.played, true);
			new CollectionSynchronizer(this.player.Duration, this.duration, true);
			new CollectionSynchronizer(this.player.Bought, this.bought, true);
			new CollectionSynchronizer(this.player.GainedLastTurn, this.gainedLastTurn, true);
			new CollectionSynchronizer(this.player.NativeVillageMat, this.nativeVillage, true);
			new CollectionSynchronizer(this.player.IslandMat, this.island, true);
			new CollectionSynchronizer(this.player.Tavern, this.tavern, true);

			this.player.PropertyChanged += new PropertyChangedEventHandler(player_PropertyChanged);
			
			this.Deck = new DeckViewModel(this.player.Deck);
			foreach (CardModel card in this.player.Hand)
			{
				CardViewModel cvm = new CardViewModel(card);
				int index = CollectionSynchronizer.LinearSearch(this.hand, cvm.CardModel);
				this.hand.Insert(index, cvm);
			}
		}

		private void player_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged(e.PropertyName);
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

	public sealed class UIPlayerStrategy : PlayerStrategy
	{
		private GameViewModel gameViewModel;
		public UIPlayerStrategy(GameViewModel gameViewModel)
			: base(gameViewModel.GameModel, new UIPlayerChooser(gameViewModel))
		{
			this.gameViewModel = gameViewModel;
		}

		public override string Name
		{
			get { return "Human"; }
		}

		public override PlayerAction GetNextAction()
		{
			return this.gameViewModel.RequestUIPlayerAction();
		}
	}

	public class UIPlayerChooser : ChooserBase
	{
		private GameViewModel gameViewModel;
		public UIPlayerChooser(GameViewModel gameViewModel)
		{
			this.gameViewModel = gameViewModel;
		}

		ManualResetEvent waitHandle = new ManualResetEvent(true);

		public override IEnumerable<CardModel> ChooseCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices)
		{
			PlayerChoice choice = this.gameViewModel.RequestUIPlayerChoice(new PlayerChoiceParameters() { SourceType = ChoiceSourceType.Card, Source = source, ChoiceText = choiceText, CardChoices = choices.Select(card => new CardViewModel(card)).ToList(), MinChoices = minChoices, MaxChoices = maxChoices });
			return choice.ChosenCards.Select(c => c.CardModel);	
		}

		public override IEnumerable<Pile> ChoosePiles(CardChoiceType choiceType, string choiceText, int minChoices, int maxChoices, IEnumerable<Pile> choices)
		{
			IEnumerable<PileViewModel> selectedChoices = null;
			PlayerChoice choice = this.gameViewModel.RequestUIPlayerChoice(new PlayerChoiceParameters() { SourceType = ChoiceSourceType.Pile, ChoiceText = choiceText, PileChoices = choices.Select(pile => new PileViewModel(pile, this.gameViewModel)).ToList(), MinChoices = minChoices, MaxChoices = maxChoices });
			selectedChoices = choice.ChosenPiles;
			return selectedChoices.Select(p => p.PileModel);
		}

		public override IEnumerable<int> ChooseEffects(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
		{
			IEnumerable<EffectViewModel> selectedChoices = null;
			List<EffectViewModel> effectChoices = new List<EffectViewModel>();
			for (int i = 0; i < choices.Length; i++)
			{
				effectChoices.Add(new EffectViewModel(choices[i], choiceDescriptions[i]));
			}
			PlayerChoice playerChoice = this.gameViewModel.RequestUIPlayerChoice(new PlayerChoiceParameters() { SourceType = ChoiceSourceType.Effect, ChoiceText = choiceText, EffectChoices = effectChoices, MinChoices = minChoices, MaxChoices = maxChoices });
			selectedChoices = playerChoice.ChosenEffects;
			return selectedChoices.Select(choice => effectChoices.IndexOf(choice));
		}

		public override IEnumerable<CardModel> Order(CardOrderType choiceType, string choiceText, IEnumerable<CardModel> choices)
		{
			IEnumerable<CardViewModel> selectedChoices = null;
			PlayerChoice choice = this.gameViewModel.RequestUIPlayerChoice(new PlayerChoiceParameters() { SourceType = ChoiceSourceType.Card, Source = ChoiceSource.None, ChoiceText = choiceText, CardChoices = choices.Select(card => new CardViewModel(card)).ToList(), MinChoices = choices.Count(), MaxChoices = choices.Count(), Order = true });
			selectedChoices = choice.ChosenCards;
			return selectedChoices.Select(c => c.CardModel);
		}
	}
}