using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dominion.CardSets;

namespace Dominion
{
	public class CardSetGroup
	{
		public ObservableCollection<CardSetViewModel> CardSets { get; private set; }
		public GameSet GameSet { get; private set; }
		public CardSetGroup(GameSet gameSet)
		{
			this.CardSets = new ObservableCollection<CardSetViewModel>();
			this.GameSet = gameSet;
		}
		public void AddCardSet(CardSet set)
		{
			this.CardSets.Add(new CardSetViewModel(set));
		}
	}

	public class CardSetsModel : NotifyingObject
	{
		public Dictionary<string, CardSetViewModel> CardSetLookup { get; private set; }
		public Dictionary<string, CardSetGroup> CardSetGroupLookup { get; private set; }
		public ObservableCollection<CardSetGroup> CardSetGroups { get; private set; }
		private CardSetViewModel selectedCardSet;
		public CardSetViewModel SelectedCardSet
		{
			get { return this.selectedCardSet; }
			set { this.selectedCardSet = value; this.OnPropertyChanged("SelectedCardSet"); }
		}

		private static CardSet[] baseCardSets = new CardSet[]
		{
			new BasicGame(),
			new BigMoney(),
			new Interaction(),
			new SizeDistortion(),
			new VillageSquare(),
			new RandomBase()
		};

		private static CardSet[] intrigueCardSets = new CardSet[]
		{
			new VictoryDance(),
			new SecretSchemes(),
			new BestWishes(),
			new RandomIntrigue(),
			new Deconstruction(),
			new HandMadness(),
			new Underlings(),
		};

		private static CardSet[] seasideCardSets = new CardSet[]
		{
			new HighSeas(),
			new BuriedTreasure(),
			new Shipwrecks(),
			new RandomSeaside(),
			new ReachForTomorrow(),
			new Repetition(),
			new GiveAndTake(),
		};

		private static CardSet[] alchemyCardSets = new CardSet[]
		{
			new ForbiddenArts(),
			new PotionMixers(),
			new ChemistryLesson(),
			new Servants(),
			new SecretResearch(),
			new PoolsToolsAndFools(),
		};

		private static CardSet[] prosperityCardSets = new CardSet[]
		{
			new Beginners(),
			new FriendlyInteractive(),
			new BigActions(),
			new BiggestMoney(),
			new TheKingsArmy(),
			new TheGoodLife(),
			new PathsToVictory(),
			new AllAlongTheWatchtower(),
			new LuckySeven(),
		};

		private static CardSet[] cornucopiaCardSets = new CardSet[]
		{
			new BountyOfTheHunt(),
			new BadOmens(),
			new TheJestersWorkshop(),
			new LastLaughs(),
			new TheSpiceOfLife(),
			new SmallVictories()
		};

		private static CardSet[] hinterlandsCardSets = new CardSet[]
		{
			new Introduction(),
			new FairTrades(),
			new Bargains(),
			new Gambits(),
			new HighwayRobbery(),
			new AdventuresAbroad(),
			new MoneyForNothing(),
			new TheDukesBall(),
			new Travelers(),
			new Diplomacy(),
			new SchemesAndDreams(),
			new WineCountry(),
			new InstantGratification(),
			new TreasureTrove(),
			new BlueHarvest(),
			new TravelingCircus()
		};

		private static CardSet[] darkAgesCardSets = new CardSet[]
		{
			new GrimParade(),
			new PlayingChessWithDeath(),
			new HighAndLow(),
			new ChivalryAndRevelry(),
			new Prophecy(),
			new Invasion(),
			new WateryGraves(),
			new Peasants(),
			new Infestations(),
			new Lamentations(),
			new OneMansTrash(),
			new HonorAmongThieves(),
			new DarkCarnival(),
			new ToTheVictor(),
			new FarFromHome(),
			new Expeditions()
		};

		private static CardSet[] guildsCardSets = new CardSet[]
		{
			new ArtsAndCrafts(),
			new CleanLiving(),
			new GildingTheLily(),
			new NameThatCard(),
			new TricksOfTheTrade(),
			new DecisionsDecisions()
		};

		private static CardSet[] adventuresCardSets = new CardSet[]
		{

		};
		
		public CardSetsModel(GameSets allowedSets)
		{
			this.CardSetGroups = new ObservableCollection<CardSetGroup>();

			if ((allowedSets & GameSets.Base) != 0)
			{
				CardSetGroup baseSet = new CardSetGroup(new GameSet(GameSets.Base, false));
				foreach (CardSet cardSet in baseCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						baseSet.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(baseSet);
			}

			if ((allowedSets & GameSets.Intrigue) != 0)
			{
				CardSetGroup intrigue = new CardSetGroup(new GameSet(GameSets.Intrigue, false));
				foreach (CardSet cardSet in intrigueCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						intrigue.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(intrigue);
			}

			if ((allowedSets & GameSets.Seaside) != 0)
			{
				CardSetGroup seaside = new CardSetGroup(new GameSet(GameSets.Seaside, false));
				foreach (CardSet cardSet in seasideCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						seaside.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(seaside);
			}

			if ((allowedSets & GameSets.Alchemy) != 0)
			{
				CardSetGroup alchemy = new CardSetGroup(new GameSet(GameSets.Alchemy, false));
				foreach (CardSet cardSet in alchemyCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						alchemy.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(alchemy);
			}

			if ((allowedSets & GameSets.Prosperity) != 0)
			{
				CardSetGroup prosperity = new CardSetGroup(new GameSet(GameSets.Prosperity, false));
				foreach (CardSet cardSet in prosperityCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						prosperity.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(prosperity);
			}

			if ((allowedSets & GameSets.Cornucopia) != 0)
			{
				CardSetGroup cornucopia = new CardSetGroup(new GameSet(GameSets.Cornucopia, false));
				foreach (CardSet cardSet in cornucopiaCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						cornucopia.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(cornucopia);
			}

			if ((allowedSets & GameSets.Hinterlands) != 0)
			{
				CardSetGroup hinterlands = new CardSetGroup(new GameSet(GameSets.Hinterlands, false));
				foreach (CardSet cardSet in hinterlandsCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						hinterlands.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(hinterlands);
			}

			if ((allowedSets & GameSets.DarkAges) != 0)
			{
				CardSetGroup darkAges = new CardSetGroup(new GameSet(GameSets.DarkAges, false));
				foreach (CardSet cardSet in darkAgesCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						darkAges.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(darkAges);
			}

			if ((allowedSets & GameSets.Guilds) != 0)
			{
				CardSetGroup guilds = new CardSetGroup(new GameSet(GameSets.Guilds, false));
				foreach (CardSet cardSet in guildsCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						guilds.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(guilds);
			}

			if((allowedSets & GameSets.Adventures) != 0)
			{
				CardSetGroup adventures = new CardSetGroup(new GameSet(GameSets.Adventures, false));
				foreach (CardSet cardSet in adventuresCardSets)
				{
					if ((cardSet.GameSet & allowedSets) == cardSet.GameSet)
					{
						adventures.AddCardSet(cardSet);
					}
				}
				this.CardSetGroups.Add(adventures);
			}

			CardSetGroup custom = new CardSetGroup(new GameSet(GameSets.None, false));
			custom.AddCardSet(new RandomAllCardSet(allowedSets));
			this.CardSetGroups.Add(custom);
			
			this.CardSetLookup = new Dictionary<string, CardSetViewModel>();
			foreach(CardSetGroup group in this.CardSetGroups)
			{
				foreach(CardSetViewModel set in group.CardSets)
				{
					this.CardSetLookup[set.CardSetName] = set;
				}
			}

			this.CardSetGroupLookup = new Dictionary<string, CardSetGroup>();
			foreach (CardSetGroup group in this.CardSetGroups)
			{
				this.CardSetGroupLookup[group.GameSet.SetName] = group;
			}
		}
	}
}
