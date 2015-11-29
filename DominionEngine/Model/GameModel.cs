using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dominion.Model.Actions;
using Dominion.Model.PileFactories;
using System.Threading;
using System.Collections.ObjectModel;
using Dominion.Model.Chooser;
using System.Diagnostics;
using DominionEngine.AI;

namespace Dominion
{
	public enum GamePhase
	{
		Action,
		Buy,
		CleanUp,
	}

	public abstract class CardModifier
	{
		public virtual int GetCoins(CardModel cardModel, int coins)
		{
			return coins;
		}

		public virtual int GetCost(CardModel cardModel, int cost)
		{
			return cost;
		}

		public virtual CardModifier Clone()
		{
			return (CardModifier)Activator.CreateInstance(this.GetType());
		}
	}

	public enum ReactionTrigger
	{
		None,
		AttackPlayed, // moat, secret chamber, horse traders, beggar
		CardGained, // trader, watchtower
		CardDiscardedOutsideCleanup, // tunnel
		OpponentGainedProvince, // fool's gold
		VictoryCardBought, // Hovel
		OwnerCardTrashed // Market Square
	}

	public class NotifyingObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[DebuggerDisplay("{Players[0].Points} {Players[1].Points}")]
	public class GameModel : INotifyPropertyChanged
	{
		public event EventHandler PlayerActionsAndPhaseUpdated;
		public event EventHandler PileBuyStatusChanged;
		public event EventHandler TurnEnded;
		public event EventHandler OnGameOver;
		public event EventHandler GameInitialized;

		private int turnCount = 0;
		public int TurnCount
		{
			get { return this.turnCount; }
			set { this.turnCount = value; this.OnPropertyChanged("TurnCount"); }
		}

		private GamePhase currentPhase;
		public GamePhase CurrentPhase
		{
			get { return this.currentPhase; }
			set { this.currentPhase = value; this.OnPropertyChanged("CurrentPhase"); }
		}

		private int currentPlayerIndex;
		public int CurrentPlayerIndex
		{
			get { return this.currentPlayerIndex; }
			set { this.currentPlayerIndex = value; this.OnPropertyChanged("CurrentPlayerIndex"); this.OnPropertyChanged("CurrentPlayer"); }
		}

		private ObservableCollection<CardModel> prizes = new ObservableCollection<CardModel>();
		public ObservableCollection<CardModel> Prizes
		{
			get { return this.prizes; }
		}

		private ObservableCollection<CardModel> blackMarket = new ObservableCollection<CardModel>();
		public ObservableCollection<CardModel> BlackMarket
		{
			get { return this.blackMarket; }
		}

		private ObservableCollection<CardModel> trash = new ObservableCollection<CardModel>();
		public ObservableCollection<CardModel> Trash
		{
			get { return this.trash; }
		}

		private bool gameStarted;
		public bool GameStarted
		{
			get { return this.gameStarted; }
			set { this.gameStarted = value; this.OnPropertyChanged("GameStarted"); }
		}

		private bool gameOver;
		public bool GameOver
		{
			get { return this.gameOver; }
			set { this.gameOver = value; this.OnPropertyChanged("GameOver"); }
		}

		private int tradeRouteCount;
		public int TradeRouteCount
		{
			get { return this.tradeRouteCount; }
			set { this.tradeRouteCount = value; this.OnPropertyChanged("TradeRouteCount"); }
		}

		private CardModel bane = new YoungWitch();
		public CardModel Bane
		{
			get { return this.bane; }
			set { this.bane = value; this.OnPropertyChanged("Bane"); }
		}

		private List<CardModifier> cardModifiers = new List<CardModifier>();
		public List<CardModifier> CardModifiers
		{
			get { return this.cardModifiers; }
		}

		private ObservableCollection<Player> players = new ObservableCollection<Player>();
		private ObservableCollection<Pile> supplyPiles = new ObservableCollection<Pile>();
		private ObservableCollection<Pile> extraPiles = new ObservableCollection<Pile>();
		private Dictionary<Type, Pile> pileMap = new Dictionary<Type, Pile>();
		private Log log;

		private bool supplyCostsModified;
		public GameResult Result { get; private set; }
		
		public bool CanBuy
		{
			get { return this.CurrentPhase == GamePhase.Buy; }
		}
		
		public bool CanPlayAction
		{
			get { return this.CurrentPhase == GamePhase.Action && this.CurrentPlayer.Actions > 0; }
		}

		public bool CanPlayTreasure
		{
			get { return this.CurrentPhase == GamePhase.Buy && this.CurrentPlayer.Bought.Count == 0; }
		}

		public bool GameHasPotions
		{
			get { return this.AllCardsInGame.Any(card => card is Potion); }
		}

		public bool GameHasPirateShip
		{
			get { return this.AllCardsInGame.Any(card => card is PirateShip); }
		}

		public bool GameHasNativeVillage
		{
			get { return this.AllCardsInGame.Any(card => card is NativeVillage); }
		}

		public bool GameHasIsland
		{
			get { return this.AllCardsInGame.Any(card => card is Island); }
		}

		public bool GameHasVPChips
		{
			get { return this.AllCardsInGame.Any(card => card is Goons || card is Bishop || card is Monument); }
		}

		public bool GameHasCoinTokens
		{
			get { return this.AllCardsInGame.Any(card => card is Baker || card is Butcher || card is CandlestickMaker || card is MerchantGuild || card is Plaza); }
		}

		public bool GameHasStash
		{
			get;
			set;
		}

		public bool GameHasContraband
		{
			get;
			set;
		}

		public bool GameHasUrchin
		{
			get;
			set;
		}

		public bool UsesShelters
		{
			get;
			set;
		}

		public bool UsesRuins
		{
			get; 
			set;
		}

		public Pile Ruins { get; private set; }


		private bool showPlayerScoreInDetails;
		public bool ShowPlayerScoreInDetails
		{
			get { return this.showPlayerScoreInDetails; }
			set { this.showPlayerScoreInDetails = value; this.OnPropertyChanged("ShowPlayerScoreInDetails"); }
		}

		public Player CurrentPlayer
		{
			get { return this.players.Count == 0 ? null : this.players[this.CurrentPlayerIndex]; }
		}

		public Player LeftOfCurrentPlayer
		{
			get { return this.PlayerLeftOf(this.CurrentPlayer); }
		}

		public Player RightOfCurrentPlayer
		{
			get { return this.PlayerRightOf(this.CurrentPlayer); }
		}

		public Player PlayerLeftOf(Player player)
		{
			int thisPlayer = this.players.IndexOf(player);
			int left = (thisPlayer + 1) % players.Count;
			return this.players[left];
		}

		public Player PlayerRightOf(Player player)
		{
			int thisPlayer = this.players.IndexOf(player);
			int right = (thisPlayer - 1 + players.Count) % players.Count;
			return this.players[right];
		}

		public ObservableCollection<Player> Players
		{
			get { return this.players; }
		}

		public ObservableCollection<Pile> SupplyPiles
		{
			get { return this.supplyPiles; }
		}

		public ObservableCollection<Pile> ExtraPiles
		{
			get { return this.extraPiles; }
		}

		public bool Has10KingdomPiles
		{
			get
			{
				int kingdomPileCount = 0;
				foreach(Pile p in this.SupplyPiles)
				{
					if (p.Card is Copper || p.Card is Silver || p.Card is Gold || p.Card is Potion || p.Card is Platinum || p.Card is Curse || p.Card is Estate || p.Card is Duchy || p.Card is Province || p.Card is Colony)
					{

					}
					else
					{
						kingdomPileCount++;
					}
				}
				return kingdomPileCount == 10;
			}
		}

		public Dictionary<Type, Pile> PileMap
		{
			get { return this.pileMap; }
		}

		private List<CardModel> allCardsInGame = new List<CardModel>();
		public IEnumerable<CardModel> AllCardsInGame
		{
			get { return this.allCardsInGame; }
		}

		public void TakePrize(CardModel prize)
		{
			this.Prizes.Remove(this.Prizes.First(card => card.Name == prize.Name));
		}

		public void AddTradeRouteToken()
		{
			this.TradeRouteCount = this.TradeRouteCount + 1;
		}

		public Log TextLog
		{
			get
			{
				return this.log;
			}
		}

		public GameModel()
		{
			this.log = new Log();
		}

		void gameState_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);
			if (e.PropertyName == "CurrentPlayerIndex")
			{
				this.OnPropertyChanged("CurrentPlayer");
			}
		}

		private void Log(string text)
		{
			this.log.WriteLine(text);
		}

		public void InitializeGameState(Kingdom kingdom)
		{
			this.InitializeGameState(kingdom, Randomizer.Next(this.players.Count));
		}

		public void InitializeGameState(Kingdom kingdom, int startingPlayer)
		{
			this.Log("Setting up game...");

			this.SupplyPiles.AddRange(CardSet.CreatePiles(this, kingdom.Cards, kingdom.Bane));

			Debug.Assert(kingdom.NumPlayers == this.Players.Count);
		
			this.allCardsInGame.AddRange(this.supplyPiles.Where(p => !p.Card.Is(CardType.Knight)).Select(p => p.Card));
			if (this.supplyPiles.Any(p => p.Card.Is(CardType.Knight)))
			{
				this.allCardsInGame.AddRange(Knights.AllKnights);
			}

			if (kingdom.UsesColonies)
			{
				this.supplyPiles.Add(new Pile(12, this, typeof(Platinum)));
				this.allCardsInGame.Add(new Platinum());
			}

			if (kingdom.UsesShelters)
			{
				this.UsesShelters = true;
				this.allCardsInGame.Add(new Hovel());
				this.allCardsInGame.Add(new OvergrownEstate());
				this.allCardsInGame.Add(new Necropolis());
			}

			if (kingdom.BlackMarketDeck != null)
			{
				this.BlackMarket.AddRange(kingdom.BlackMarketDeck);
				this.allCardsInGame.AddRange(kingdom.BlackMarketDeck);
			}

			if (kingdom.Bane != null)
			{
				Debug.Assert(this.supplyPiles.Any(p => p.Card.Name == kingdom.Bane.Name));
				this.Bane = kingdom.Bane;
				this.allCardsInGame.Add(this.Bane);
			}

			if (this.allCardsInGame.Any(card => card.Is(CardType.Looter)))
			{
				this.UsesRuins = true;
				this.Ruins = new Pile(10 * (kingdom.NumPlayers - 1), this, typeof(AbandonedMine));
				this.supplyPiles.Add(this.Ruins);
				this.allCardsInGame.Add(new AbandonedMine());
				this.allCardsInGame.Add(new RuinedLibrary());
				this.allCardsInGame.Add(new RuinedMarket());
				this.allCardsInGame.Add(new RuinedVillage());
				this.allCardsInGame.Add(new Survivors());
			}

			if (this.allCardsInGame.Any(card => card is Pillage || card is BanditCamp || card is Marauder))
			{
				this.extraPiles.Add(new Pile(15, this, typeof(Spoils)));
				this.allCardsInGame.Add(new Spoils());
			}

			if (this.allCardsInGame.Any(card => card is Hermit))
			{
				this.extraPiles.Add(new Pile(10, this, typeof(Madman)));
				this.allCardsInGame.Add(new Madman());
			}

			if (this.allCardsInGame.Any(card => card is Urchin))
			{
				this.extraPiles.Add(new Pile(10, this, typeof(Mercenary)));
				this.allCardsInGame.Add(new Mercenary());
			}

			if (this.AllCardsInGame.Any(card => card is Tournament))
			{
				this.Prizes.Add(new BagOfGold());
				this.Prizes.Add(new Diadem());
				this.Prizes.Add(new Followers());
				this.Prizes.Add(new Princess());
				this.Prizes.Add(new TrustySteed());
				this.allCardsInGame.AddRange(this.Prizes);
			}

			this.CurrentPlayerIndex = startingPlayer;

			this.supplyPiles.Add(new Pile(kingdom.VictoryCardCount, this, typeof(Estate)));
			this.supplyPiles.Add(new Pile(kingdom.VictoryCardCount, this, typeof(Duchy)));
			this.supplyPiles.Add(new Pile(kingdom.VictoryCardCount, this, typeof(Province)));
			this.allCardsInGame.Add(new Estate());
			this.allCardsInGame.Add(new Duchy());
			this.allCardsInGame.Add(new Province());

			if (kingdom.UsesColonies)
			{
				this.supplyPiles.Add(new Pile(kingdom.VictoryCardCount, this, typeof(Colony)));
				this.allCardsInGame.Add(new Colony());
			}

			this.GameHasStash = this.allCardsInGame.Any(c => c is Stash);
			this.GameHasContraband = this.allCardsInGame.Any(c => c is Contraband);
			this.GameHasUrchin = this.allCardsInGame.Any(c => c is Urchin);
			

			switch (kingdom.NumPlayers)
			{
				case 2:
					this.supplyPiles.Add(new Pile(46, this, typeof(Copper)));
					this.supplyPiles.Add(new Pile(10, this, typeof(Curse)));
					break;
				case 3:
					this.supplyPiles.Add(new Pile(39, this, typeof(Copper)));
					this.supplyPiles.Add(new Pile(20, this, typeof(Curse)));
					break;
				case 4:
					this.supplyPiles.Add(new Pile(32, this, typeof(Copper)));
					this.supplyPiles.Add(new Pile(30, this, typeof(Curse)));
					break;
				default:
					break;
			}

			this.supplyPiles.Add(new Pile(40, this, typeof(Silver)));
			this.supplyPiles.Add(new Pile(30, this, typeof(Gold)));
			this.allCardsInGame.Add(new Copper());
			this.allCardsInGame.Add(new Silver());
			this.allCardsInGame.Add(new Gold());
			this.allCardsInGame.Add(new Curse());
			if (this.AllCardsInGame.Any(card => card.CostsPotion))
			{
				this.supplyPiles.Add(new Pile(16, this, typeof(Potion)));
				this.allCardsInGame.Add(new Potion());
			}

			if (this.allCardsInGame.Any(c => c is TradeRoute))
			{
				foreach (Pile pile in this.supplyPiles)
				{
					if (pile.Card.Is(CardType.Victory))
					{
						pile.TradeRouteCount = 1;
					}
					else
					{
						pile.TradeRouteCount = 0;
					}
				}
			}

			int maxCost = (from pile in this.supplyPiles select pile.GetCost()).Max();
			this.supplyPiles = new ObservableCollection<Pile>(this.supplyPiles.OrderBy(y =>
			{
				int cost = 0;
				// basic treasures: 0-10
				if (y.Card is Copper)
				{
					return 0;
				}
				else if (y.Card is Silver)
				{
					return 1;
				}
				else if (y.Card is Gold)
				{
					return 2;
				}
				else if (y.Card is Platinum)
				{
					cost = 3;
				}
				else if (y.Card is Potion)
				{
					return 4;
				}
				else if (y.Card.Is(CardType.Curse)) // curse: 50
				{
					cost = 50;
				}
				else if (y.Card is Estate) // basic VPs: 51-60
				{
					return 51;
				}
				else if (y.Card is Duchy) // basic VPs: 51-60
				{
					return 52;
				}
				else if (y.Card is Province) // basic VPs: 51-60
				{
					return 53;
				}
				else if (y.Card is Colony) // basic VPs: 51-60
				{
					return 54;
				}
				else // kingdom: 101+
				{
					cost = 101 + y.GetCost();
				}

				if (y.CostsPotion)
				{
					cost += 20;
				}
				return cost;
			}));
			this.InitializePileMap();

			this.GameOver = false;
			for (int i = 0; i < this.players.Count;i++)
			{
				this.players[i].InitializePlayer(kingdom.StartingDecks[i]);
			}
			this.OnPropertyChanged("Piles");
			this.OnPropertyChanged("Players");
			this.OnPropertyChanged("CurrentPlayer");
			this.OnPropertyChanged("GameHasPotions");
			this.OnPropertyChanged("GameHasPirateShip");
			this.OnPropertyChanged("GameHasNativeVillage");
			this.OnPropertyChanged("GameHasVPChips");
			this.OnPropertyChanged("GameHasCoinTokens");
			this.CurrentPhase = GamePhase.Action;
			this.TradeRouteCount = 0;
			this.GameStarted = true;

			this.OnPropertyChanged("CanPlayAction");
			this.OnPropertyChanged("CanPlayTreasure");
			this.OnPropertyChanged("CanBuy");

			if (this.GameInitialized != null)
			{
				this.GameInitialized(this, EventArgs.Empty);
			}
		}

		private void InitializePileMap()
		{
			foreach (Pile pile in this.supplyPiles)
			{
				this.pileMap[pile.Card.GetType()] = pile;
			}
			foreach (Pile pile in this.extraPiles)
			{
				this.pileMap[pile.Card.GetType()] = pile;
			}
		}

		public event EventHandler TurnStarted;
		public void StartTurn()
		{
			this.TextLog.StartTurn();
			if (this.TurnStarted != null)
			{
				this.TurnStarted(this, EventArgs.Empty);
			}
		}

		public void PlayGame()
		{
			this.PlayGame(-1);
		}

		public void PlayGame(int maxTurns)
		{
			this.CurrentPlayer.TurnCount++;
			this.Log(this.CurrentPlayer.Name + " turn " + this.CurrentPlayer.TurnCount + " begins.");
			this.CurrentPlayer.EnterPhase(GamePhase.Action);
			this.GameLoop(maxTurns);
		}

		public void GameLoop(int maxTurns)
		{
			while (!this.GameOver)
			{
				// for testing
				if (this.CurrentPlayer.TurnCount == maxTurns)
				{
					return;
				}
				if (this.CurrentPhase == GamePhase.Action && !this.CurrentPlayer.Hand.Any(card => card.Is(CardType.Action)))
				{
					this.AdvancePhase();
				}
				PlayerAction action = this.CurrentPlayer.Strategy.GetNextAction();
				this.HandlePlayerAction(action);
			}
		}

		public void HandlePlayerAction(PlayerAction action)
		{
			switch (action.ActionType)
			{
				case ActionType.PlayCard:
					if (action.Card != null)
					{
						this.Play(action.Card);
					}
					if (this.CurrentPhase == GamePhase.Action && this.CurrentPlayer.Actions == 0)
					{
						this.AdvancePhase();
					}
					break;
				case ActionType.PlayBasicTreasures:
					if (this.CurrentPhase == GamePhase.Action)
					{
						this.AdvancePhase();
					}
					this.PlayBasicTreasures();
					break;
				case ActionType.PlayCoinTokens:
					string[] choices = new string[this.CurrentPlayer.CoinTokens + 1];
					for (int i = 0; i <= this.CurrentPlayer.CoinTokens; i++)
					{
						choices[i] = (i).ToString();
					}
					int choice = this.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.PlayCoinTokens, "Choose number of coin tokens to play", choices, choices);
					this.CurrentPlayer.PlayCoinTokens(choice);
					break;
				case ActionType.CleanupCard:
					if (action.Card != null)
					{
						this.CurrentPlayer.DoCleanup(action.Card);
					}
					else // cleanup all
					{
						foreach (CardModel card in this.CurrentPlayer.Cleanup.ToArray())
						{
							card.OnCleanup(this);
						}
					}
					if (this.CurrentPlayer.Cleanup.Count == 0)
					{
						this.AdvancePhase();
					}
					break;
				case ActionType.BuyCard:
					this.Buy(action.Pile);
					if (this.CurrentPlayer.Buys == 0)
					{
						this.AdvancePhase();
					}
					break;
				case ActionType.EnterBuyPhase:
					if (this.CurrentPhase == GamePhase.Action)
					{
						this.AdvancePhase();
					}
					break;
				case ActionType.EndTurn:
					if (this.CurrentPhase == GamePhase.CleanUp)
					{
						this.AdvancePhase();
					}
					else
					{
						if (this.CurrentPhase == GamePhase.Action)
						{
							this.AdvancePhase();
						}
						if (this.CurrentPhase == GamePhase.Buy)
						{
							this.AdvancePhase();
						}
					}

					break;
				default:
					break;
			}
			if (this.PileBuyStatusChanged != null)
			{
				this.PileBuyStatusChanged(this, EventArgs.Empty);
			}
		}

		public void AddCardModifier(CardModifier cardModifier)
		{
			this.supplyCostsModified = true;
			this.CardModifiers.Add(cardModifier);
			this.UpdateSupplyPileCosts();
		}

		public void UpdateSupplyPileCosts()
		{
			if (this.supplyCostsModified)
			{
				this.supplyCostsModified = false;
				foreach (Pile pile in this.SupplyPiles)
				{
					pile.UpdateCost();
				}
			}
		}

		private void UpdatePlayerActionsAndPhase()
		{
			if (this.PlayerActionsAndPhaseUpdated != null)
			{
				this.PlayerActionsAndPhaseUpdated(this, EventArgs.Empty);
			}
		}

		public int GetCoins(CardModel cardModel)
		{
			int coins = cardModel.Coins;

			foreach (CardModifier cardModifier in this.CardModifiers)
			{
				coins = cardModifier.GetCoins(cardModel, coins);
			}

			return coins;
		}

		public int GetCost(CardModel cardModel)
		{
			int cost = cardModel.GetBaseCost();
			if (cardModel is Peddler && this.CurrentPhase == GamePhase.Buy)
			{
				cost = Math.Max(0, cost - 2 * this.CurrentPlayer.Played.Count(card => card.Is(CardType.Action)));
			}
			
			foreach (CardModifier cardModifier in this.CardModifiers)
			{
				cost = cardModifier.GetCost(cardModel, cost);
			}
	
			if (this.CurrentPlayer.FerryPile != null)
			{
				if (this.CurrentPlayer.FerryPile.Card.Name == cardModel.ThisAsTrashTarget.Name || this.CurrentPlayer.FerryPile.Card.Is(CardType.Ruins) && cardModel.Is(CardType.Ruins) || this.CurrentPlayer.FerryPile.Card.Is(CardType.Knight) && cardModel.Is(CardType.Knight))
				{
					cost = Math.Max(0, cost - 2);
				}
			}
			return cost;
		}

		public int GetCost(Pile pile)
		{
			return this.GetCost(pile.TopCard ?? pile.Card);
		}

		private static string[] wineMerchantChoices = new string[] { "Yes", "No" };
		public void AdvancePhase()
		{
			if (this.GameStarted && !this.GameOver)
			{
				switch (this.CurrentPhase)
				{
					case GamePhase.Action:
						this.CurrentPhase = GamePhase.Buy;
						break;

					case GamePhase.Buy:
						
						foreach (CardModel wineMerchant in this.CurrentPlayer.Tavern.Where(c => c.Name == WineMerchant.Name).ToArray())
						{
							if (this.CurrentPlayer.Coin >= 2)
							{
								int choice = this.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.WineMerchant, "You may pay $2 to discard Wine Merchant", wineMerchantChoices, wineMerchantChoices);
								if (choice == 0)
								{
									this.CurrentPlayer.AddActionCoin(-2);
									this.CurrentPlayer.Tavern.Remove(wineMerchant);
									this.CurrentPlayer.DiscardCard(wineMerchant);
								}		
							}
						}
						
						this.CurrentPhase = GamePhase.CleanUp;
						this.CleanUpPhase();
						break;

					case GamePhase.CleanUp:
						this.EndTurnPhase();
						if (!this.GameOver)
						{
							this.CurrentPhase = GamePhase.Action;
						}
						break;
				}

				if (!this.GameOver)
				{
					this.CurrentPlayer.EnterPhase(this.CurrentPhase);
					// no cleanup effect
					if (this.CurrentPhase == GamePhase.CleanUp && this.CurrentPlayer.Cleanup.Count == 0)
					{
						this.AdvancePhase();
					}
				}

				Pile peddlers = null;
				if (this.PileMap.TryGetValue(typeof(Peddler), out peddlers))
				{
					peddlers.UpdateCost();
				}
				this.OnPropertyChanged("CanPlayAction");
				this.OnPropertyChanged("CanPlayTreasure");
				this.OnPropertyChanged("CanBuy");
			}
		}

		private void CheckGameEnd()
		{
			int emptyPiles = 0;

			foreach (Pile pile in this.SupplyPiles)
			{
				if (pile.Count <= 0)
				{
					if (pile.Card is Province || pile.Card is Colony || ++emptyPiles >= 3)
					{
						this.GameOver = true;
						break;
					}
				}
			}
			if (this.GameOver)
			{
				this.Result = new GameResult(this);
				foreach (Player p in this.Players)
				{
					p.OnGameEnd();
				}
				foreach (Player winner in this.Result.Winners)
				{
					this.Log(winner.Name + " wins!");
				}
				this.Log("Final scores:");
				this.Log("------------------------");
				string log = string.Empty;
				foreach (PlayerResult result in this.Result.Results)
				{
					if (log.Length != 0)
					{
						log += ", ";
					}
					log += result.Player.Name + ": " + result.Score + " points in " + result.Turns + " turns";
				}
				this.Log(log);
				this.log.WriteLine();
				this.Log("Deck Contents");
				this.Log("------------------------");
				foreach (Player player in this.players)
				{
					this.log.Write(player.Name + ": ");
					this.log.WriteSortedCards(player.AllCardsInDeck);
					this.log.WriteLine();
				}
				if (this.OnGameOver != null)
				{
					this.OnGameOver(this, EventArgs.Empty);
				}
			}
		}

		private void CleanUpPhase()
		{
			this.CurrentPlayer.DiscardHardAndCleanup();
		}

		private void EndTurnPhase()
		{
			bool wasPossessionTurn = false;
			if (this.CurrentPlayer.IsPossessionTurn)
			{
				List<CardModel> trashed = this.CurrentPlayer.PossessionTrash.ToList();
				if (trashed.Count > 0)
				{
					this.log.Write(this.CurrentPlayer.Name);
					this.log.WriteLine(" returns trashed cards to his deck");
				}
				this.CurrentPlayer.PossessionTrash.Clear();
				foreach (CardModel card in trashed)
				{
					this.CurrentPlayer.Discard.Add(card);
				}
				this.CurrentPlayer.IsPossessionTurn = false;
				this.CurrentPlayer.PossessionTurns--;
				if (this.CurrentPlayer.PossessionTurns == 0)
				{
					wasPossessionTurn = true;
				}
			}
			this.CurrentPlayer.DrawNewHand();
			if (this.CurrentPlayer.SaveEventCard != null)
			{
				this.CurrentPlayer.Hand.Add(this.CurrentPlayer.SaveEventCard);
				this.CurrentPlayer.SaveEventCard = null;
			}
			this.CurrentPlayer.HasTravellingFairEffect = false;
			foreach (Player player in players)
			{
				player.HasHauntedWoodsEffect.Remove(this.CurrentPlayer);
				player.HasSwampHagEffect[this.CurrentPlayer] = 0;
			}

			if (this.CardModifiers.Count > 0)
			{
				this.CardModifiers.Clear();
				this.supplyCostsModified = true;
			}
			this.UpdateSupplyPileCosts();
			if (this.GameHasContraband)
			{
				foreach (Pile pile in this.supplyPiles)
				{
					pile.Contrabanded = false;
				}
			}
			this.CheckGameEnd();
			if (!this.GameOver)
			{
				if (this.CurrentPlayer.HasOutpostTurn && !this.CurrentPlayer.IsOutpostTurn)
				{
					if (!this.log.IsSuppressingLogging)
					{
						this.Log(this.CurrentPlayer.Name + " turn " + this.CurrentPlayer.TurnCount + "(outpost) begins.");
					}
				}
				else if (wasPossessionTurn)
				{
					if (!this.log.IsSuppressingLogging)
					{
						this.Log(this.CurrentPlayer.Name + " turn " + this.CurrentPlayer.TurnCount + " begins");
					}
				}
				else
				{
					if (this.CurrentPlayer.PossessionTurns > 0)
					{
						this.CurrentPlayer.IsPossessionTurn = true;
						if (!this.log.IsSuppressingLogging)
						{
							this.Log(this.CurrentPlayer.Name + " turn " + this.CurrentPlayer.TurnCount + " begins(possessed by " + this.PlayerRightOf(this.CurrentPlayer).Name + ").");
						}
					}
					else
					{
						this.CurrentPlayerIndex = (this.CurrentPlayerIndex + 1) % this.players.Count;
						this.CurrentPlayer.TurnCount++;
						if (this.CurrentPlayer.PossessionTurns > 0)
						{
							this.CurrentPlayer.IsPossessionTurn = true;
							if (!this.log.IsSuppressingLogging)
							{
								this.Log(this.CurrentPlayer.Name + " turn " + this.CurrentPlayer.TurnCount + " begins(possessed by " + this.PlayerRightOf(this.CurrentPlayer).Name + ").");
							}
						}
						else
						{
							if (!this.log.IsSuppressingLogging)
							{
								this.log.Write(this.CurrentPlayer.Name);
								this.log.Write(" turn ");
								this.log.Write(this.CurrentPlayer.TurnCount.ToString());
								this.log.WriteLine(" begins.");
							}
						}
					}
				}

				this.TurnCount = this.TurnCount + 1;
				this.OnPropertyChanged("CurrentPlayer");
			}
			if (this.TurnEnded != null)
			{
				this.TurnEnded(this, EventArgs.Empty);
			}
		}

		public void Buy(Pile pile)
		{
			if (this.GameStarted && !this.GameOver && pile != null) 
			{
				this.CurrentPlayer.BuyCard(pile);
				this.OnPropertyChanged("CanBuy");
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

		public void Play(CardModel cardModel)
		{
			if (this.GameStarted && !this.GameOver)
			{
				if (cardModel != null && cardModel.Is(CardType.Action) && this.CanPlayAction)
				{
					this.CurrentPlayer.PlayAction(cardModel);
					this.OnPropertyChanged("CanPlayAction");
				}
				else if (cardModel != null && cardModel.Is(CardType.Treasure) && this.CanPlayTreasure)
				{
					this.CurrentPlayer.PlayTreasure(cardModel);
				}
			}
		}

		public void PlayBasicTreasures()
		{
			if (this.GameStarted && !this.GameOver)
			{
				this.CurrentPlayer.PlayBasicTreasures();
			}
		}

		private static string[] marketSquareChoices = new string[] { "Discard", "Keep" };
		internal void TrashCard(CardModel chosenCard, Player owner)
		{
			this.Trash.Add(chosenCard);
			chosenCard.OnTrash(this, owner);
			foreach (CardModel card in owner.Hand.Where(c => c is MarketSquare).ToArray())
			{
				int choice = owner.Chooser.ChooseOneEffect(EffectChoiceType.DiscardMarketSquare, "You may discard Market Square to gain a Gold", marketSquareChoices, marketSquareChoices);
				if (choice == 0)
				{
					owner.DiscardCard(card);
					owner.GainCard(typeof(Gold));
				}
			}
		}

		public GameModel CloneAndPlay(PlayerAction playerAction, bool keepHandInfo, Func<GameModel, GameModel, Player, BaseAIStrategy> strategyFactory)
		{
			GameModel clone = this.Clone(keepHandInfo, strategyFactory);
			PlayerAction clonedAction = new PlayerAction();
			clonedAction.ActionType = playerAction.ActionType;
			switch (playerAction.ActionType)
			{
				case ActionType.BuyCard:
					clonedAction.Pile = clone.SupplyPiles.First(p => p.Name == playerAction.Pile.Name);
					break;
				case ActionType.CleanupCard:
					clonedAction.Card = clone.CurrentPlayer.Cleanup.First(c => c.Name == playerAction.Card.Name);
					break;
				case ActionType.PlayCard:
					clonedAction.Card = clone.CurrentPlayer.Hand.First(c => c.Name == playerAction.Card.Name);
					break;
			}

			clone.HandlePlayerAction(clonedAction);
			return clone;
		}

		public GameModel Clone(bool keepHandInfo, Func<GameModel, GameModel, Player, BaseAIStrategy> strategyFactory)
		{
			GameModel clone = new GameModel();
			clone.allCardsInGame.AddRange(this.AllCardsInGame.Select(c => c.Clone()));
			clone.Bane = this.Bane.Clone();
			clone.BlackMarket.AddRange(this.BlackMarket.Select(c => c.Clone()));
			clone.CardModifiers.AddRange(this.CardModifiers.Select(cm => cm.Clone()));
			clone.currentPhase = this.currentPhase;
			clone.currentPlayerIndex = this.currentPlayerIndex;
			clone.extraPiles.AddRange(this.extraPiles.Select(p => p.Clone(clone)));
			clone.GameHasStash = this.GameHasStash;
			clone.GameHasContraband = this.GameHasContraband;
			clone.GameHasUrchin = this.GameHasUrchin;
			clone.UsesRuins = this.UsesRuins;
			clone.UsesShelters = this.UsesShelters;
			clone.gameOver = this.gameOver;
			clone.gameStarted = this.gameStarted;
			clone.players.AddRange(this.players.Select(p => p.Clone(clone)));
			foreach (Player player in clone.Players)
			{
				player.SetStrategy(strategyFactory(this, clone, player));
				if (!keepHandInfo)
				{
					if (player != clone.CurrentPlayer)
					{
						int handCount = player.Hand.Count;
						player.Deck.Populate(player.Hand);
						player.Deck.Shuffle(true);
						player.Hand.Clear();
						player.Draw(handCount);
					}
					else
					{
						player.Deck.Shuffle(true);
					}
				}
			}
			clone.prizes.AddRange(this.prizes.Select(p => p.Clone()));
			clone.supplyPiles.AddRange(this.supplyPiles.Select(p => p.Clone(clone)));
			clone.Ruins = clone.SupplyPiles.FirstOrDefault(p => p.Card.Is(CardType.Ruins));
			clone.tradeRouteCount = this.tradeRouteCount;
			clone.trash.AddRange(this.trash.Select(c => c.Clone()));
			clone.turnCount = this.turnCount;
			clone.InitializePileMap();
			return clone;
		}
	}
}
