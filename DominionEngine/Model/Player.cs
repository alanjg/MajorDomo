#define NOINTRO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Dominion.Model.Chooser;
using Dominion.Model.Actions;

namespace Dominion
{
	public class ObservableCollection2<T> : ObservableCollection<T>
	{
		public new void Clear()
		{
			if(this.Count != 0)
			{
				base.Clear();
			}
		}
	}

	public enum GainLocation
	{
		Default,
		InHand,
		TopOfDeck
	}

	public class Player : INotifyPropertyChanged
	{
		private GameModel gameModel;
		public GameModel GameModel { get { return this.gameModel; } }
		private Log log;

		private PlayerStrategy strategy;
		public void SetStrategy(PlayerStrategy playerStrategy)
		{
			this.strategy = playerStrategy;
			this.strategy.Player = this;
		}

		public ObservableCollection2<CardModel> Hand { get; private set; }
		public ObservableCollection2<CardModel> Discard { get; private set; }
		public ObservableCollection2<CardModel> Cleanup { get; private set; }
		public ObservableCollection2<CardModel> Played { get; private set; }
		public ObservableCollection2<CardModel> Duration { get; private set; }
		public ObservableCollection2<CardModel> Bought { get; private set; }
		public ObservableCollection2<CardModel> GainedLastTurn { get; private set; }
		public ObservableCollection2<CardModel> NativeVillageMat { get; private set; }
		public ObservableCollection2<CardModel> IslandMat { get; private set; }
		public ObservableCollection2<CardModel> SetAsideHaven { get; private set; }
		public ObservableCollection2<CardModel> SetAsideHorseTraders { get; private set; }
		public ObservableCollection2<CardModel> SetAsidePrince { get; private set; }
		public ObservableCollection2<CardModel> SetAsidePrincePlay { get; private set; }
		public ObservableCollection2<CardModel> PossessionTrash { get; private set; }
		public ObservableCollection2<CardModel> Tavern { get; private set; }

		private int actions;
		public int Actions
		{
			get { return this.actions; }
			set { if (this.actions != value) { this.actions = value; this.OnPropertyChanged("Actions"); } }
		}

		private int buys;
		public int Buys
		{
			get { return this.buys; }
			set { if (this.buys != value) { this.buys = value; this.OnPropertyChanged("Buys"); } }
		}

		private int coin;
		public int Coin
		{
			get { return this.coin; }
			set { if (this.coin != value) { this.coin = value; this.OnPropertyChanged("Coin"); } }
		}

		private int coinTokens;
		public int CoinTokens
		{
			get { return this.coinTokens; }
			set { if (this.coinTokens != value) { this.coinTokens = value; this.OnPropertyChanged("CoinTokens"); } }
		}

		private int potions;
		public int Potions
		{
			get { return this.potions; }
			set { if (this.potions != value) { this.potions = value; this.OnPropertyChanged("Potions"); } }
		}

		private int vpChips;
		public int VPChips
		{
			get { return this.vpChips; }
			set { if (this.vpChips != value) { this.vpChips = value; this.OnPropertyChanged("VPChips"); } }
		}

		private int pirateShipTokens;
		public int PirateShipTokens
		{
			get { return this.pirateShipTokens; }
			set { this.pirateShipTokens = value; this.OnPropertyChanged("PirateShipTokens"); }
		}

		private bool hasOutpostTurn;
		public bool HasOutpostTurn
		{
			get { return this.hasOutpostTurn; }
			set { this.hasOutpostTurn = value; this.OnPropertyChanged("HasOutpostTurn"); }
		}

		private bool isOutpostTurn;
		public bool IsOutpostTurn
		{
			get { return this.isOutpostTurn; }
			set { this.isOutpostTurn = value; this.OnPropertyChanged("IsOutpostTurn"); }
		}

        private int possessionTurns;
        public int PossessionTurns
        {
            get { return this.possessionTurns; }
            set { this.possessionTurns = value; this.OnPropertyChanged("PossessionTurns"); }
        }

		private bool isPossessionTurn;
		public bool IsPossessionTurn
		{
			get { return this.isPossessionTurn; }
			set { this.isPossessionTurn = value; this.OnPropertyChanged("IsPossessionTurn"); }
		}

		private bool hasUsedAlms;
		public bool HasUsedAlms
		{
			get { return this.hasUsedAlms; }
			set { this.hasUsedAlms = value; this.OnPropertyChanged("HasUsedAlms"); }
		}

		private bool hasMinusOneCoinToken;
		public bool HasMinusOneCoinToken
		{
			get { return this.hasMinusOneCoinToken; }
			set { this.hasMinusOneCoinToken = value; this.OnPropertyChanged("HasMinusOneCoinToken"); }
		}

		private bool hasUsedBorrow;
		public bool HasUsedBorrow
		{
			get { return this.hasUsedBorrow; }
			set { this.hasUsedBorrow = value; this.OnPropertyChanged("HasUsedBorrow"); }
		}

		private bool hasMinusOneCardToken;
		public bool HasMinusOneCardToken
		{
			get { return this.hasMinusOneCardToken; }
			set { this.hasMinusOneCardToken = value; this.OnPropertyChanged("HasMinusOneCardToken"); }
		}

		private int deferredCoin;
		public int DeferredCoin
		{
			get { return this.deferredCoin; }
			set { this.deferredCoin = value; this.OnPropertyChanged("DeferredCoin"); }
		}
		private int expeditionCount;
		public int ExpeditionCount
		{
			get { return this.expeditionCount; }
			set { this.expeditionCount = value; this.OnPropertyChanged("ExpeditionCount"); }
		}

		private bool journeyTokenIsFaceUp;
		public bool JourneyTokenIsFaceUp
		{
			get { return this.journeyTokenIsFaceUp; }
			set { this.journeyTokenIsFaceUp = value; this.OnPropertyChanged("JourneyTokenIsFaceUp"); }
		}

		public bool HasUsedInheritance { get; set; }

		public Pile FerryPile { get; set; }
		public Pile EstatePile { get; set; }
		public Pile ActionPile { get; set; }
		public Pile BuyPile { get; set; }
		public Pile CardPile { get; set; }
		public Pile CoinPile { get; set; }
		public HashSet<Player> HasHauntedWoodsEffect { get; private set; }
		public Dictionary<Player, int> HasSwampHagEffect { get; private set; }
		public bool HasTravellingFairEffect { get; set; }

		public CardModel SaveEventCard { get; set; }
		public bool HasUsedSaveEvent { get; set; }

		private int schemeCounter = 0;
		public ObservableCollection<CardModel> Schemed { get; private set; }

		public Deck Deck
		{
			get;
			private set;
		}

		public int TurnCount { get; set; }
		public int PlayedActions { get; private set; }
		public IEnumerable<CardModel> AllCardsInDeck
		{
			get
			{
				return this.Deck.Union(this.Played.Union(this.Discard.Union(this.Hand.Union(this.NativeVillageMat.Union(this.Duration.Union(this.IslandMat.Union(this.SetAsideHorseTraders.Union(this.SetAsideHaven.Union(this.SetAsidePrince.Union(this.SetAsidePrincePlay.Union(this.Tavern)))))))))));
			}
		}

		public PlayerStrategy Strategy
		{
			get
			{
				if (this.IsPossessionTurn)
				{
					return this.gameModel.PlayerRightOf(this).Strategy;
				}
				return this.strategy;
			}
		}

		public PlayerStrategy OriginalStrategy
		{
			get
			{
				return this.strategy;
			}
		}

		public IChooser Chooser
		{
			get
			{
                if (this.IsPossessionTurn)
                {
					return this.gameModel.PlayerRightOf(this).Chooser;
                }
				return this.strategy.Chooser;
			}
		}

		public string Name
		{
			get;
			private set;
		}

		public bool HasActions
		{
			get
			{
				return this.Hand.Any(card => card.Is(CardType.Action));
			}
		}

		public int Points
		{
			get
			{
				int points = this.VPChips;
				points += this.Deck.GetPoints(this);
				points += this.Played.Points(this);
				points += this.Discard.Points(this);
				points += this.Hand.Points(this);
				points += this.NativeVillageMat.Points(this);
				points += this.Duration.Points(this);
				points += this.IslandMat.Points(this);
				points += this.Tavern.Points(this);
				points += this.SetAsideHorseTraders.Points(this);
				points += this.SetAsideHaven.Points(this);
				points += this.SetAsidePrince.Points(this);
				points += this.SetAsidePrincePlay.Points(this);
				
				return points;
			}
		}

		public bool IsImmuneToAttacks
		{
			get;
			set;
		}

		public Player(string name, PlayerStrategy strategy, GameModel gameModel)
		{
			this.Name = name;
			this.Hand = new ObservableCollection2<CardModel>();
			this.Discard = new ObservableCollection2<CardModel>();
			this.Cleanup = new ObservableCollection2<CardModel>();
			this.Played = new ObservableCollection2<CardModel>();
			this.Duration = new ObservableCollection2<CardModel>();
			this.Bought = new ObservableCollection2<CardModel>();
			this.GainedLastTurn = new ObservableCollection2<CardModel>();
			this.NativeVillageMat = new ObservableCollection2<CardModel>();
			this.IslandMat = new ObservableCollection2<CardModel>();
			this.Tavern = new ObservableCollection2<CardModel>();
			this.SetAsideHaven = new ObservableCollection2<CardModel>();
			this.SetAsideHorseTraders = new ObservableCollection2<CardModel>();
			this.SetAsidePrince = new ObservableCollection2<CardModel>();
			this.SetAsidePrincePlay = new ObservableCollection2<CardModel>();
			this.PossessionTrash = new ObservableCollection2<CardModel>();
			this.Schemed = new ObservableCollection2<CardModel>();
			if (strategy != null)
			{
				this.SetStrategy(strategy);
			}

			this.Deck = new Deck(this);
			this.log = gameModel.TextLog;
			this.gameModel = gameModel;
			this.HasHauntedWoodsEffect = new HashSet<Player>();
			this.HasSwampHagEffect = new Dictionary<Player, int>();
		}

		public void DiscardCard(CardModel card)
		{
			this.Log(this.Name + " discards " + card.Name + ".");
			this.Hand.Remove(card);
			this.Discard.Add(card);
			if (card.Is(CardType.Reaction) && card.ReactionTrigger == ReactionTrigger.CardDiscardedOutsideCleanup && this.gameModel.CurrentPhase != GamePhase.CleanUp)
			{
				CardModel reaction = this.Chooser.ChooseZeroOrOneCard(CardChoiceType.Tunnel, "You may reveal Tunnel to gain a gold", ChoiceSource.None, new CardModel[] { card } );
				if (reaction is Tunnel)
				{
					this.GainCard(typeof(Gold));
				}
			}
		}

		public void PutDeckInDiscard()
		{
			this.Log(this.Name + " puts the deck in the discard.");
			for (int i = this.Deck.Count - 1; i >= 0; i--)
			{
				this.Discard.Add(this.Deck.Draw());
			}
		}

		private static string[] guideChoices = new string[] { "Yes", "No" };
		private static string[] teacherTokenTypeChoices = new string[] { "Card", "Action", "Buy", "Coin" };
		public void EnterPhase(GamePhase phase)
		{
			if (phase == GamePhase.Action)
			{
				this.gameModel.StartTurn();
				foreach(Player player in this.gameModel.Players)
				{
					player.Strategy.OnTurnStart(this);
				}
				this.IsOutpostTurn = false;
				if (this.HasOutpostTurn)
				{
					this.HasOutpostTurn = false;
					this.IsOutpostTurn = true;
				}
				this.Coin = 0;
				this.Potions = 0;
				this.Buys = 1;
				this.Actions = 1;
				this.PlayedActions = 0;
				this.schemeCounter = 0;
				this.Schemed.Clear();
				this.IsImmuneToAttacks = false;

				foreach (CardModel card in this.Tavern.ToArray())
				{
					if (card.Name == Guide.Name)
					{
						int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.Guide, "You may call Guide to discard your hand and draw 5 cards", guideChoices, guideChoices);
						if (choice == 0)
						{
							this.Tavern.Remove(card);
							this.Played.Add(card);
							this.DiscardHand();
							this.Draw(5);
						}
					}
					if(card.Name == Ratcatcher.Name)
					{
						int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.Ratcatcher, "You may call Ratcatcher to trash a card from your hand", guideChoices, guideChoices);
						if (choice == 0)
						{
							this.Tavern.Remove(card);
							this.Played.Add(card);
							CardModel cardToTrash = this.Chooser.ChooseOneCard(CardChoiceType.Trash, "Trash a card from your hand", ChoiceSource.FromHand, this.Hand);
							if (cardToTrash != null)
							{
								this.Trash(cardToTrash);
							}
						}
					}
					if (card.Name == Transmogrify.Name)
					{
						int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.Transmogrify, "You may call Transmogrify	 to trash a card from your hand", guideChoices, guideChoices);
						if (choice == 0)
						{
							this.Tavern.Remove(card);
							this.Played.Add(card);
							CardModel cardToTrash = this.Chooser.ChooseOneCard(CardChoiceType.Trash, "Trash a card from your hand", ChoiceSource.FromHand, this.Hand);
							if (cardToTrash != null)
							{
								this.Trash(cardToTrash);
								Pile newChoice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to $" + (gameModel.GetCost(cardToTrash) + 1).ToString(),
									gameModel.SupplyPiles.Where(pile => gameModel.GetCost(pile) <= gameModel.GetCost(cardToTrash) + 1 && (cardToTrash.CostsPotion || !pile.CostsPotion) && pile.Count > 0));

								if (newChoice != null)
								{
									gameModel.CurrentPlayer.GainCard(newChoice);
								}
							}
						}
					}
					if(card.Name == Teacher.Name)
					{
						int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.Teacher, "You may call Teacher to move your +1 Card, +1 Action, +1 Buy, or +$1 token to an Action supply pile you have no tokens on.", guideChoices, guideChoices);
						if (choice == 0)
						{
							this.Tavern.Remove(card);
							this.Played.Add(card);
							int effectChoice = this.Chooser.ChooseOneEffect(EffectChoiceType.TeacherTokenType, "Choose which token to move", teacherTokenTypeChoices, teacherTokenTypeChoices);
							Pile targetPile = this.Chooser.ChooseOnePile(CardChoiceType.TeacherTokenPile, "Choose the target pile", gameModel.SupplyPiles.Where(p => p.Card.Is(CardType.Action) && this.ActionPile != p && this.BuyPile != p && this.CardPile != p && this.CoinPile != p));
							if (targetPile != null)
							{
								switch (effectChoice)
								{
									case 0:
										this.CardPile = targetPile;
										break;
									case 1:
										this.ActionPile = targetPile;
										break;
									case 2:
										this.BuyPile = targetPile;
										break;
									case 3:
										this.CoinPile = targetPile;
										break;
								}
							}
						}
					}
				}

				this.AddActionCoin(this.DeferredCoin);
				foreach (CardModel cardModel in this.Duration)
				{
					this.PlayDuration(this.gameModel, cardModel);
					while (cardModel.DurationPlayMultiplier > 1)
					{
						cardModel.DurationPlayMultiplier--;
						this.PlayDuration(this.gameModel, cardModel);
					}
					cardModel.DurationPlayMultiplier = 0;
				}
				foreach (CardModel cardModel in this.SetAsideHorseTraders)
				{
					this.Hand.Add(cardModel);
					this.Draw();
				}
				foreach (CardModel cardModel in this.SetAsideHaven)
				{
					this.Hand.Add(cardModel);
				}
				if (this.SetAsidePrincePlay.Count > 0)
				{
					List<CardModel> princePlay = new List<CardModel>();
					princePlay.AddRange(this.SetAsidePrincePlay);
					this.SetAsidePrincePlay.Clear();
					while(princePlay.Count > 0)
					{
						CardModel next = this.Strategy.Chooser.ChooseOneCard(CardChoiceType.PrincePlayOrder, "Pick a card to play from Prince", ChoiceSource.None, princePlay);
						this.Play(next, false, false, " with Prince");
						princePlay.Remove(next);
					}
				}
				this.SetAsideHorseTraders.Clear();
				this.SetAsideHaven.Clear();
				this.GainedLastTurn.Clear();
			}
			if (phase == GamePhase.CleanUp)
			{
				int toScheme = Math.Min(this.schemeCounter, this.Played.Count(p => p.Is(CardType.Action)));
				for (int i = 0; i < toScheme; i++)
				{
					CardModel choice = this.Chooser.ChooseZeroOrOneCard(CardChoiceType.Scheme, "Choose an action to scheme", ChoiceSource.InPlay, this.Played.Where(p => p.Is(CardType.Action) && !this.Schemed.Contains(p)));
					if (choice != null)
					{
						this.Schemed.Add(choice);
					}
				}
			}
		}

		private void PlayTreasures()
		{
            foreach (CardModel card in this.Hand.ToList())
            {
				if (card.Is(CardType.Treasure))
				{
					this.gameModel.Play(card);
				}
			}
		}

		public void DoCleanup(CardModel card)
		{
			card.OnCleanup(this.gameModel);
			if (this.Cleanup.Contains(card))
			{
				this.Cleanup.Remove(card);
				this.Played.Add(card);
			}
		}

		public void GainActions(int count)
		{
			this.Actions += count;
		}

		public void AddActionCoin(int coin)
		{
			if (this.HasMinusOneCoinToken)
			{
				coin = Math.Max(0, coin - 1);
				this.HasMinusOneCoinToken = false;
			}
			this.Coin += coin;
		}

		public void GainBuys(int count)
		{
			this.Buys += count;
		}

		public void AddVPChips(int count)
		{
			this.VPChips += count;
		}

		public void AddCoinTokens(int count)
		{
			this.CoinTokens += count;
		}

		public void AddPirateShipTreasureToken()
		{
			this.PirateShipTokens++;
		}

		public void PutNativeVillageCardsIntoHand()
		{
			foreach (CardModel card in this.NativeVillageMat)
			{
				this.PutInHand(card);
			}
			this.NativeVillageMat.Clear();
		}

		public void PutCardOnNativeVillage()
		{
			CardModel card = this.DrawCard();
			if (card != null)
			{
				this.NativeVillageMat.Add(card);
			}
		}

		public void PutOnIsland(CardModel island, CardModel card)
		{
			this.IslandMat.Add(island);
			if (card != null)
			{
				this.IslandMat.Add(card);
			}
		}

		public void PutOnTavern(CardModel card)
		{			
			if (card != null)
			{
				this.Tavern.Add(card);
			}
		}

		public void SetAsideForHorseTraders(CardModel card)
		{
			this.SetAsideHorseTraders.Add(card);
		}

		public void SetAsideForHaven(CardModel card)
		{
			this.SetAsideHaven.Add(card);
		}

		public void AddSchemeEffect()
		{
			this.schemeCounter++;
		}

		public void OnGameEnd()
		{
			foreach (CardModel card in this.Duration)
			{
				card.OnGameEnd(this.gameModel, this);
			}
		}

		public void InitializePlayer(List<CardModel> startingDeck)
		{
			this.VPChips = 0;
			this.CoinTokens = this.gameModel.AllCardsInGame.Any(c => c is Baker) ? 1 : 0;
			this.Deck.Populate(startingDeck, shuffle: false);
			this.DrawNewHand();
		}

		private void Log(string text)
		{
			this.log.WriteLine(text);
		}

		public void DiscardHand()
		{
			foreach(CardModel card in this.Hand.ToList())
			{
				this.DiscardCard(card);
			}
		}

		public void DiscardHardAndCleanup()
		{
			this.Discard.AddRange(this.Hand);
			this.Hand.Clear();

			List<CardModel> champions = new List<CardModel>();
			foreach (CardModel durationPlayed in this.Duration.ToArray())
			{
				if (durationPlayed.Name != Champion.Name && durationPlayed.Name != Hireling.Name)
				{
					durationPlayed.OnDiscardedFromPlay(this.gameModel);
					durationPlayed.OnRemovedFromPlay(this.gameModel);
				}
				else
				{
					champions.Add(durationPlayed);
				}
			}

			this.Discard.AddRange(this.Duration);
			this.Duration.Clear();
			this.Duration.AddRange(champions);

			List<CardModel> duration = new List<CardModel>();
			List<CardModel> cleanup = new List<CardModel>();
			foreach (CardModel card in this.Played.ToList())
			{
				if (card.Is(CardType.Duration))
				{
					duration.Add(card);
				}
				else
				{
					if (card.HasCleanupEffect(this.gameModel))
					{
						cleanup.Add(card);
					}
				}
			}

			foreach (CardModel card in duration)
			{
				this.Duration.Add(card);
				this.Played.Remove(card);
			}

			foreach (CardModel card in cleanup)
			{
				this.Cleanup.Add(card);
				this.Played.Remove(card);
			}
		}

		public void DrawNewHand()
		{
			this.Schemed.Reverse();
			foreach (CardModel card in this.Schemed)
			{
				if (this.Played.Contains(card))
				{
					this.Played.Remove(card);
					this.Deck.PlaceOnTop(card);
					card.OnRemovedFromPlay(this.gameModel);
				}
				else if (this.Cleanup.Contains(card))
				{
					this.Cleanup.Remove(card);
					this.Deck.PlaceOnTop(card);
					card.OnRemovedFromPlay(this.gameModel);
				}
			}

			foreach (CardModel played in this.Played.ToArray())
			{
				played.OnDiscardedFromPlay(this.gameModel);
				played.OnRemovedFromPlay(this.gameModel);
				if (played.PlayedWithPrinceSource != null && !this.Played.Contains(played))
				{
					this.SetAsidePrincePlay.Remove(played);
					played.PlayedWithPrinceSource = null;
				}	
			}

			this.Discard.AddRange(this.Played);
			
			foreach (CardModel played in this.Cleanup.ToArray())
			{
				played.OnDiscardedFromPlay(this.gameModel);
				played.OnRemovedFromPlay(this.gameModel);
				if (played.PlayedWithPrinceSource != null && !this.Cleanup.Contains(played))
				{
					this.SetAsidePrincePlay.Remove(played);
					played.PlayedWithPrinceSource = null;
				}
			}
			this.Discard.AddRange(this.Cleanup);

			this.Played.Clear();
			this.Bought.Clear();
			this.Cleanup.Clear();

			int cardsToDraw = this.HasOutpostTurn ? 3 : 5;
			cardsToDraw += 2 * this.ExpeditionCount;
			this.ExpeditionCount = 0;
			this.Draw(cardsToDraw);
		}

		// returns true if the reaction provides immunity to the attack
		private bool ReactToAttack(CardModel attack)
		{
			bool immune = this.Duration.Any(card => card.Name == Lighthouse.Name || card.Name == Champion.Name);
			if(immune)
			{
				this.Log(this.Name + " has a lighthouse in play and is immune to the attack.");
			}
			IEnumerable<CardModel> reactions = this.Hand.Where(c => c.Is(CardType.Reaction) && c.ReactionTrigger == ReactionTrigger.AttackPlayed && (!(c is Moat) || !immune));

			if (reactions.Any())
			{
				CardModel reaction = null;
				do
				{
					reaction = this.Chooser.ChooseZeroOrOneCard(CardChoiceType.ReactToAttack, attack, "You may reveal a reaction", ChoiceSource.FromHand, reactions);
					if (reaction != null)
					{
						this.Log(this.Name + " reveals " + reaction.Name);
						immune |= reaction.ReactToAttack(gameModel, this);
					}
					reactions = this.Hand.Where(c => c.Is(CardType.Reaction) && c.ReactionTrigger == ReactionTrigger.AttackPlayed && (!(c is Moat) || !immune));

				} while (reaction != null && reactions.Any());
			}
			return immune;
		}

		public void Draw(int n)
		{
			for (int i = 0; i < n; i++)
			{
				Draw();
			}
		}

		public void Draw()
		{
			CardModel card = this.DrawCard();
			if (card != null)
			{
				this.Hand.Add(card);
			}
		}

		public void DrawTo(int n)
		{
			int toDraw = n - this.Hand.Count;
			if (toDraw > 0)
			{
				Draw(toDraw);
			}
		}

		public CardModel DrawCard()
		{
			if (this.Deck.Count == 0 && this.Discard.Count > 0)
			{
				this.Log(this.Name + " is reshuffling...");
				this.Deck.Populate(this.Discard);
				this.Discard.Clear();
			}
			if (this.Deck.Count > 0)
			{
				return this.Deck.Draw();
			}
			return null;
		}

		public CardModel DrawCardFromBottom()
		{
			if (this.Deck.Count == 0 && this.Discard.Count > 0)
			{
				this.Log(this.Name + " is reshuffling...");
				this.Deck.Populate(this.Discard);
				this.Discard.Clear();
			}
			if (this.Deck.Count > 0)
			{
				return this.Deck.DrawFromBottom();
			}
			return null;
		}

		public List<CardModel> DrawCards(int n)
		{
			List<CardModel> cards = new List<CardModel>();
			for (int i = 0; i < n; i++)
			{
				CardModel card = this.DrawCard();
				if (card != null)
				{
					cards.Add(card);
				}
				else
				{
					break;
				}
			}
			return cards;
		}

		public void DiscardTo(int n)
		{
			int toDiscard = this.Hand.Count - n;
			if (toDiscard > 0)
			{
				this.DiscardCards(toDiscard);
			}
		}

		public void DiscardCards(int n)
		{
			n = Math.Min(n, this.Hand.Count);
			IEnumerable<CardModel> discards = this.Chooser.ChooseSeveralCards(CardChoiceType.Discard, "Discard " + n + " cards", ChoiceSource.FromHand, n, n, this.Hand);
			foreach (CardModel discard in discards.ToList())
			{
				this.DiscardCard(discard);
			}
		}

		private static string[] hovelChoices = new string[] { "Yes", "No" };

		public void BuyCard(Pile pile)
		{
			this.BuyCard(pile, null, true);
		}

		public void BuyBlackMarketCard(CardModel card)
		{
			this.BuyCard(null, card, false);
		}

		private void BuyCard(Pile pile, CardModel card, bool useBuy)
		{
			int coinCost, potionCost;
			if(pile != null)
			{
				coinCost = pile.Cost;
				potionCost = pile.CostsPotion ? 1 : 0;
				card = pile.DrawCard();
			}
			else
			{
				coinCost = this.gameModel.GetCost(card);
				potionCost = card.CostsPotion ? 1 : 0;
			}
			
			Debug.Assert((!useBuy || this.Buys > 0) && this.Coin >= coinCost && this.Potions >= potionCost);

			if ((!useBuy || this.Buys > 0) && this.Coin >= coinCost && this.Potions >= potionCost)
			{
				this.Log(this.Name + " buys " + card.Name + ".");
				this.GainCard(card, pile, GainLocation.Default, true);
				
				bool useTalisman = pile != null && this.gameModel.GetCost(card) <= 4 && pile.Count > 0 && !card.Is(CardType.Victory);
				int talismanCount = 0;
				int goonsCount = 0;
				bool useHoard = card.Is(CardType.Victory);
				int hoardCount = 0;
				int hagglerCount = 0;
				int merchantGuildCount = 0;
				foreach (CardModel playedCard in this.Played)
				{
					if (useTalisman && playedCard is Talisman)
					{
						talismanCount++;
					}
					if (playedCard is Goons)
					{
						goonsCount++;
					}
					if (useHoard && playedCard is Hoard)
					{
						hoardCount++;
					}
					if (playedCard is Haggler)
					{
						hagglerCount++;
					}
					if (playedCard is MerchantGuild)
					{
						merchantGuildCount++;
					}
				}
				
				if (pile != null)
				{
					if (useTalisman)
					{
						for (int i = 0; i < talismanCount; i++)
						{
							if (pile.TopCard.Name == card.Name && pile.Count > 0)
							{
								this.GainCard(pile);							
							}
						}
					}

					for (int i = 0; i < pile.EmbargoCount; i++)
					{
						this.GainCard(typeof(Curse));
					}
				}

				this.Coin -= coinCost;
				if (potionCost != 0)
				{
					this.Potions -= potionCost;
				}

				if (useBuy)
				{
					this.Buys -= 1;
				}

				if (goonsCount > 0)
				{
					this.VPChips += goonsCount;
				}

				if (card.Is(CardType.Victory))
				{
					for (int i = 0; i < hoardCount; i++)
					{
						this.GainCard(typeof(Gold));
					}
					if (this.gameModel.UsesShelters)
					{
						foreach (CardModel c in this.Hand.Where(c => c is Hovel).ToArray())
						{
							int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.Hovel, "Do you want to trash Hovel?", hovelChoices, hovelChoices);
							if (choice == 0)
							{
								this.Trash(c);
							}
						}
					}
				}

				for (int i = 0; i < hagglerCount; i++)
				{
					IEnumerable<Pile> piles = this.gameModel.SupplyPiles.Where(p => 
						(this.gameModel.GetCost(p) < this.gameModel.GetCost(card) && (card.CostsPotion || !p.CostsPotion) ||
						this.gameModel.GetCost(p) == this.gameModel.GetCost(card) && !p.CostsPotion && card.CostsPotion) 
						&& p.Count > 0 && !p.TopCard.Is(CardType.Victory));
					if (piles.Any())
					{
						Pile choice = this.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card from Haggler", piles);
						this.GainCard(choice);
					}
				}

				if (merchantGuildCount != 0)
				{
					this.AddCoinTokens(merchantGuildCount);
				}
				
				card.OnBuy(this.gameModel);
				this.Bought.Add(card);

				if (this.HasHauntedWoodsEffect.Any())
				{
					IEnumerable<CardModel> cards = this.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put your hand on top of your deck in any order", this.Hand).ToArray();
					this.Hand.Clear();
					foreach(CardModel c in cards)
					{
						this.Deck.PlaceOnTop(c);
					}
				}
				foreach (KeyValuePair<Player, int> kvp in this.HasSwampHagEffect)
				{
					if (kvp.Key != this)
					{
						for (int i = 0; i < kvp.Value; i++)
						{
							this.GainCard(typeof(Curse));
						}
					}
				}
			}
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

		public bool HasBasicTreasures
		{
			get
			{
				return this.Hand.Any(card => card.Is(CardType.Treasure) && !card.AffectsTreasurePlayOrder);
			}
		}

		public void PlayAction(CardModel cardModel)
		{
			this.Play(cardModel, true, true);
		}

		public void PlayTreasure(CardModel cardModel)
		{
			this.Play(cardModel, false, true);
		}
		public void PlayTreasure(CardModel cardModel, int cardIndex)
		{
			this.Play(cardModel, false, true, cardIndex);
		}

		public void PlayCoinTokens(int count)
		{
			this.log.WriteLine(this.Name + " plays " + count + " coin tokens.");
			this.AddCoinTokens(-count);
			this.Coin += count;
		}

		public void PlayBasicTreasures()
		{			
			List<CardModel> cards = new List<CardModel>();
			using (this.log.SuppressLogging())
			{
				for (int i = this.Hand.Count - 1; i >= 0; i--)
				{
					if (this.Hand[i].Is(CardType.Treasure) && !this.Hand[i].AffectsTreasurePlayOrder)
					{
						cards.Add(this.Hand[i]);
						this.PlayTreasure(this.Hand[i], i);
					}
				}
			}
			this.log.Write(this.Name);
			this.log.Write(" plays ");
			this.log.WriteSortedCards(cards);
			this.log.WriteLine();
		}

		private static string[] urchinChoices = new string[] { "Trash", "Nothing" };
		private static string[] urchinChoiceDescriptions = new string[] { "Trash Urchin", "Do not trash Urchin" };
		public void Play(CardModel cardModel, bool consumeActions, bool removeFromHand)
		{
			this.Play(cardModel, consumeActions, removeFromHand, -1, string.Empty);
		}

		public void Play(CardModel cardModel, bool consumeActions, bool removeFromHand, string playModifierText)
		{
			this.Play(cardModel, consumeActions, removeFromHand, -1, playModifierText);
		}

		public void Play(CardModel cardModel, bool consumeActions, bool removeFromHand, int cardIndex)
		{
			this.Play(cardModel, consumeActions, removeFromHand, cardIndex, string.Empty);
		}

		private static string[] royalCarriageChoices = new string[] { "Yes", "No" };
		public void Play(CardModel cardModel, bool consumeActions, bool removeFromHand, int cardIndex, string playModifierText)
		{
			if (!this.log.IsSuppressingLogging)
			{
				this.Log(this.Name + " plays " + cardModel.Name + playModifierText + ".");
			}
			int champions = this.Duration.Count(c => c.Name == Champion.Name);
			this.GainActions(champions);
			if (this.ActionPile != null)
			{
				if (this.ActionPile.Card.Name == cardModel.ThisAsTrashTarget.Name || this.ActionPile.Card.Is(CardType.Ruins) && cardModel.Is(CardType.Ruins) || this.ActionPile.Card.Is(CardType.Knight) && cardModel.Is(CardType.Knight))
				{
					this.GainActions(1);
				}
			}

			if (this.BuyPile != null)
			{
				if (this.BuyPile.Card.Name == cardModel.ThisAsTrashTarget.Name || this.BuyPile.Card.Is(CardType.Ruins) && cardModel.Is(CardType.Ruins) || this.BuyPile.Card.Is(CardType.Knight) && cardModel.Is(CardType.Knight))
				{
					this.GainBuys(1);
				}
			}

			if (this.CoinPile != null)
			{
				if (this.CoinPile.Card.Name == cardModel.ThisAsTrashTarget.Name || this.CoinPile.Card.Is(CardType.Ruins) && cardModel.Is(CardType.Ruins) || this.CoinPile.Card.Is(CardType.Knight) && cardModel.Is(CardType.Knight))
				{
					this.AddActionCoin(1);
				}
			}

			if (this.CardPile != null)
			{
				if (this.CardPile.Card.Name == cardModel.ThisAsTrashTarget.Name || this.CardPile.Card.Is(CardType.Ruins) && cardModel.Is(CardType.Ruins) || this.CardPile.Card.Is(CardType.Knight) && cardModel.Is(CardType.Knight))
				{
					this.Draw(1);
				}
			}

			cardModel.BeforePlay(this.gameModel);
			if (removeFromHand)
			{
				if(cardIndex == -1)
				{
					this.Hand.Remove(cardModel);
				}
				else
				{
					this.Hand.RemoveAt(cardIndex);
				}				
				this.Played.Add(cardModel);
			}

			if (this.gameModel.GameHasUrchin)
			{
				if (cardModel.Is(CardType.Attack))
				{
					foreach (CardModel card in this.Played.Where(c => c != cardModel && c.Name == Urchin.Name).ToArray())
					{
						int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.TrashUrchin, "You may trash Urchin to gain a Mercenary", urchinChoices, urchinChoiceDescriptions);
						if (choice == 0)
						{
							this.Trash(card);
							this.GainCard(typeof(Mercenary));
						}
					}
				}
			}

			if (consumeActions)
			{
				this.Actions -= 1;
			}

			if (cardModel.Is(CardType.Action))
			{
				this.PlayedActions++;
			}

			this.Draw(cardModel.Cards);			

			int coin = this.gameModel.GetCoins(cardModel);
			if (coin != 0 && this.HasMinusOneCoinToken)
			{
				coin--;
				this.HasMinusOneCoinToken = false;
			}
			this.Coin += coin;
			this.Potions += cardModel.Potions;
			this.Actions += cardModel.Actions;
			this.Buys += cardModel.Buys;

			cardModel.Play(gameModel);

			if (cardModel.Is(CardType.Attack))
			{
				List<Player> attackable = new List<Player>();
				foreach (Player player in this.gameModel.Players.Where(p => p != this.gameModel.CurrentPlayer))
				{
					bool immuneToAttack = player.ReactToAttack(cardModel);
					if (!immuneToAttack)
					{
						attackable.Add(player);
					}
				}

				cardModel.PlayAttack(gameModel, attackable);
				cardModel.PlayPostAttack(gameModel);
			}
			if (this.Played.Contains(cardModel))
			{
				foreach (CardModel card in this.Tavern.ToArray())
				{
					if (card.Name == RoyalCarriage.Name)
					{
						int callChoice = this.Chooser.ChooseOneEffect(EffectChoiceType.RoyalCarriage, "You may call Royal Carriage to replay " + cardModel.Name, royalCarriageChoices, royalCarriageChoices);
						if(callChoice == 0)
						{
							this.Tavern.Remove(card);
							this.Played.Add(card);
							this.Play(cardModel, false, false);
						}
					}
				}
			}
		}

		public void PlayDuration(GameModel gameModel, CardModel cardModel)
		{
			this.Log(this.Name + " receives the effects of the duration card " + cardModel.Name + ".");

			cardModel.PlayDuration(gameModel);
		}

		private static string[] watchtowerChoices = new string[] { "TopDeck", "Trash" };
		private static string[] watchtowerChoiceDescriptions = new string[] { "Put on top of your deck", "Trash" };

		private static string[] royalSealChoices = new string[] { "TopDeck", "Nothing" };
		private static string[] royalSealChoiceDescriptions = new string[] { "Put on top of your deck", "Do nothing" };

		public void GainCard(CardModel card, Pile pile, GainLocation gainLocation = GainLocation.Default, bool bought = false)
		{
			if (this.IsPossessionTurn)
			{
				this.Log(this.Name + " gains " + card.Name + "(it is gained by " + this.gameModel.PlayerRightOf(this).Name + " instead)");
				this.gameModel.PlayerRightOf(this).GainCard(card, pile);
				return;
			}

			IEnumerable<CardModel> reactions = this.Hand.Where(c => c.Is(CardType.Reaction) && c.ReactionTrigger == ReactionTrigger.CardGained && !(card is Silver && c is Trader));
			if (pile == null)
			{
				if (!this.gameModel.PileMap.TryGetValue(card.GetType(), out pile))
				{
					if (card.Is(CardType.Ruins))
					{
						pile = this.gameModel.Ruins;
					}
					else if (card is Knights)
					{
						this.gameModel.PileMap.TryGetValue(typeof(Knights), out pile);
					}
					else
					{
						pile = this.gameModel.SupplyPiles.Union(this.gameModel.ExtraPiles).Where(p => p.Card.GetType() == card.GetType()).FirstOrDefault();
					}
				}
			}
			bool gainedThisCard = true;
			if (reactions.Any())
			{
				CardModel reaction = this.Chooser.ChooseZeroOrOneCard(CardChoiceType.ReactToGain, "You may reveal a reaction", ChoiceSource.FromHand, reactions);
				if (reaction != null)
				{
					this.Log(this.Name + " reveals " + reaction.Name);
				}
				if (reaction is Watchtower)
				{
					int putOnDeckChoice = this.Chooser.ChooseOneEffect(EffectChoiceType.Watchtower, card, this.Name + " reveals Watchtower", watchtowerChoices, watchtowerChoiceDescriptions);
					if (putOnDeckChoice == 0)
					{
						gainLocation = GainLocation.TopOfDeck;							
					}
					else
					{
						gainedThisCard = false;
						this.Trash(card);
					}
				}
				else if (reaction is Trader)
				{
					gainedThisCard = false;
					if (pile != null)
					{
						pile.PutCardOnPile(card);
					}
					this.GainCard(typeof(Silver));
				}
			}

			if (!bought && gainedThisCard)
			{
				this.Log(this.Name + " gains " + card.Name + ".");
			}			
			
			if (gainedThisCard && gainLocation != GainLocation.InHand && this.Played.Any(played => played is RoyalSeal))
			{
				int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.RoyalSeal, card, this.Name + " has a Royal Seal in play", royalSealChoices, royalSealChoiceDescriptions);
				if (choice == 0)
				{
					gainLocation = GainLocation.TopOfDeck;
				}
			}
			if (gainedThisCard && gainLocation != GainLocation.InHand && gainLocation != GainLocation.TopOfDeck && this.HasTravellingFairEffect)
			{
				int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.RoyalSeal, card, "You may put it on top of your deck", royalSealChoices, royalSealChoiceDescriptions);
				if (choice == 0)
				{
					gainLocation = GainLocation.TopOfDeck;
				}
			}
			if (gainedThisCard)
			{
				this.GainedLastTurn.Add(card);
				switch (gainLocation)
				{
					case GainLocation.Default:
						this.Discard.Add(card);
						break;

					case GainLocation.TopOfDeck:
						this.Deck.PlaceOnTop(card);
						break;

					case GainLocation.InHand:
						this.Hand.Add(card);
						break;
				}
				this.OnGainedCard(card);
			}
			
			if (card.Is(CardType.Victory) && gainedThisCard)
			{
				if (pile != null)
				{
					if (pile.TradeRouteCount == 1)
					{
						pile.TradeRouteCount = 0;
					}
				}
			}
			
			if (gainedThisCard)
			{
				card.OnGain(this.gameModel, this);
			}
		}

		public CardModel GainCard(Pile pile, GainLocation gainLocation = GainLocation.Default, bool bought = false)
		{
			CardModel card = pile.DrawCard();
			if (card != null)
			{
				this.GainCard(card, pile, gainLocation, bought);
			}
			return card;
		}

		public CardModel GainCard(Type cardType, GainLocation gainLocation = GainLocation.Default, bool bought = false)
		{
			Pile pile;
			if (this.gameModel.PileMap.TryGetValue(cardType, out pile))
			{
				return this.GainCard(pile, gainLocation, bought);
			}
			else
			{
				return null;
			}
		}

		public void PutInHand(CardModel newCard)
		{
			this.Log(this.Name + " puts " + newCard.Name + " into his hand.");
			this.Hand.Add(newCard);
		}

		public void GainOutpostTurn()
		{
			this.HasOutpostTurn = true;
		}

        public void GainPossessionTurn()
        {
            this.PossessionTurns++;
        }

		private static string[] duplicateChoices = new string[] { "Yes", "No" };
		public void OnGainedCard(CardModel card)
		{
			this.strategy.OnThisPlayerGainedCard(card);
			foreach (Player player in this.gameModel.Players)
			{
				if (player != this)
				{
					player.strategy.OnOtherPlayerGainedCard(this, card);
				}
			}
			if (gameModel.GetCost(card) <= 6 && !card.CostsPotion)
			{
				foreach (Duplicate duplicate in this.Tavern.Where(c => c.Name == Duplicate.Name))
				{
					int choice = this.Chooser.ChooseOneEffect(EffectChoiceType.Duplicate, "You may call Duplicate", duplicateChoices, duplicateChoices);
					if (choice == 0)
					{
						Pile pile = gameModel.PileMap.FirstOrDefault(kvp => kvp.Value.Name == card.Name).Value;
						if (pile != null)
						{
							this.GainCard(pile);
						}
						this.Tavern.Remove(duplicate);
						this.Played.Add(duplicate);
					}
				}
			}
			this.OnPropertyChanged("Points");
		}

		public void OnTrashedCard(CardModel card)
		{
			this.OnPropertyChanged("Points");
		}

		public void OnPassedCard()
		{
			this.OnPropertyChanged("Points");
		}

		public void Trash(CardModel card)
		{
			string log = this.Name + " trashes " + card.Name;
			if (card.PlayedAsBandOfMisfitsSource != null)
			{
				card = card.PlayedAsBandOfMisfitsSource;
			}
			this.Hand.Remove(card);
			bool removed = this.Played.Remove(card);
			
			if (this.IsPossessionTurn)
			{
				log += "(it is set aside)";
				this.Log(log);
				this.PossessionTrash.Add(card);
			}
			else
			{
				this.Log(log);
				this.gameModel.TrashCard(card, this);
				this.OnTrashedCard(card);
			}
			if (removed)
			{
				card.OnRemovedFromPlay(this.gameModel);
			}
		}
		
		public void RemoveFromHand(CardModel card)
		{
			this.Hand.Remove(card);
		}

		public void RemoveFromPlayed(CardModel card)
		{
			this.Played.Remove(card);
		}

		public void RemoveFromDiscard(CardModel card)
		{
			this.Discard.Remove(card);
		}

		public void RemoveFromCleanUp(CardModel card)
		{
			this.Cleanup.Remove(card);
		}

		public void RevealHand()
		{
			this.gameModel.TextLog.Write(this.Name + " reveals his hand: ");
			this.gameModel.TextLog.WriteSortedCards(this.Hand);
			this.gameModel.TextLog.WriteLine();
		}

		public void RevealCardFromHand(CardModel card)
		{
			this.gameModel.TextLog.Write(this.Name + " reveals a card from hand: ");
			this.gameModel.TextLog.WriteLine(card.Name);
		}

		public void RevealCard(CardModel card)
		{
			this.gameModel.TextLog.Write(this.Name + " reveals a card: ");
			this.gameModel.TextLog.WriteLine(card.Name);
		}

		public void RevealCards(List<CardModel> cards)
		{
			this.gameModel.TextLog.Write(this.Name + " reveals: ");
			this.gameModel.TextLog.WriteSortedCards(cards);
			this.gameModel.TextLog.WriteLine();
		}

		public Player Clone(GameModel gameModel)
		{
			Player clone = new Player(this.Name, null, gameModel);
			clone.actions = this.actions;
			clone.Bought.AddRange(this.Bought.Select(c => c.Clone()));
			clone.buys = this.buys;
			clone.Cleanup.AddRange(this.Cleanup.Select(c => c.Clone()));
			clone.coin = this.coin;
			clone.coinTokens = this.coinTokens;
			clone.Deck = this.Deck.Clone(clone);
			clone.Discard.AddRange(this.Discard.Select(c => c.Clone()));
			clone.Duration.AddRange(this.Duration.Select(c => c.Clone()));
			clone.GainedLastTurn.AddRange(this.GainedLastTurn.Select(c => c.Clone()));
			clone.Hand.AddRange(this.Hand.Select(c => c.Clone()));
			clone.hasOutpostTurn = this.hasOutpostTurn;
			clone.IslandMat.AddRange(this.IslandMat.Select(c => c.Clone()));
			clone.Tavern.AddRange(this.Tavern.Select(c => c.Clone()));
			clone.isOutpostTurn = this.isOutpostTurn;
			clone.isPossessionTurn = this.isPossessionTurn;
			clone.NativeVillageMat.AddRange(this.NativeVillageMat.Select(c => c.Clone()));
			clone.pirateShipTokens = this.pirateShipTokens;
			clone.Played.AddRange(this.Played.Select(c => c.Clone()));
			clone.PlayedActions = this.PlayedActions;
			clone.PossessionTrash.AddRange(this.PossessionTrash.Select(c => c.Clone()));
			clone.possessionTurns = this.possessionTurns;
			clone.potions = this.potions;
			clone.schemeCounter = this.schemeCounter;
			
			clone.SetAsideHaven.AddRange(this.SetAsideHaven.Select(c => c.Clone()));
			clone.SetAsideHorseTraders.AddRange(this.SetAsideHorseTraders.Select(c => c.Clone()));
			clone.SetAsidePrince.AddRange(this.SetAsidePrince.Select(c => c.Clone()));

			// need to map from cards in played
			foreach (CardModel c in this.SetAsidePrincePlay)
			{
				int whichPrince = this.SetAsidePrince.IndexOf(c.PlayedWithPrinceSource);
				Debug.Assert(whichPrince != -1);
				int which = this.Played.IndexOf(c);
				if (which != -1)
				{
					clone.SetAsidePrincePlay.Add(clone.Played[which]);
					clone.Played[which].PlayedWithPrinceSource = this.SetAsidePrince[whichPrince];
				}
				else
				{
					which = this.Cleanup.IndexOf(c);
					if (which != -1)
					{
						clone.SetAsidePrincePlay.Add(c);
						clone.Played[which].PlayedWithPrinceSource = this.SetAsidePrince[whichPrince];
					}
					else
					{
						Debug.Assert(false);
					}
				}
			}
			clone.TurnCount = this.TurnCount;
			clone.vpChips = this.vpChips;

			// need to map from cards in played
			foreach(CardModel c in this.Schemed)
			{
				int which = this.Played.IndexOf(c);
				if (which != -1)
				{
					clone.Schemed.Add(clone.Played[which].Clone());
				}
				else
				{
					which = this.Duration.IndexOf(c);
					if(which != -1)
					{
						clone.Schemed.Add(clone.Duration[which].Clone());
					}
					else
					{
						Debug.Assert(false, "Cannot find schemed card");
					}					
				}
			}				

			return clone;
		}
	}

	public static class Extensions
	{
		public static void AddRange<T>(this ObservableCollection<T> thing, IEnumerable<T> items) where T : class
		{
			foreach (T item in items)
			{
				thing.Add(item);
			}
		}
	}
}
