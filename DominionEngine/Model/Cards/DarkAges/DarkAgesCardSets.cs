using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using Dominion.Model.Actions;

namespace Dominion.CardSets
{
	public sealed class GrimParade : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Armory(),
			new BandOfMisfits(),
			new Catacombs(),
			new Cultist(),
			new Forager(),
			new Fortress(),
			new Knights(),
			new MarketSquare(),
			new Procession(),
			new HuntingGrounds()
		};

		public GrimParade()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Grim Parade"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges; }
		}
	}

	public sealed class PlayingChessWithDeath : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new BanditCamp(),
			new Graverobber(),
			new JunkDealer(),
			new Mystic(),
			new Pillage(),
			new Rats(),
			new Sage(),
			new Scavenger(),
			new Storeroom(),
			new Vagrant()
		};

		public PlayingChessWithDeath()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Playing Chess With Death"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges; }
		}
	}

	public sealed class HighAndLow : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Hermit(),
			new HuntingGrounds(),
			new Mystic(),
			new PoorHouse(),
			new WanderingMinstrel(),
			new Cellar(),
			new Moneylender(),
			new ThroneRoom(),
			new Witch(),
			new Workshop()
		};

		public HighAndLow()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "High and Low"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Base; }
		}
	}

	public sealed class ChivalryAndRevelry : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Altar(),
			new Knights(),
			new Rats(),
			new Scavenger(),
			new Squire(),
			new Festival(),
			new Gardens(),
			new Laboratory(),
			new Library(),
			new Remodel()
		};

		public ChivalryAndRevelry()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Chivalry and Revelry"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Base; }
		}
	}

	public sealed class Prophecy : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Armory(),
			new Ironmonger(),
			new Mystic(),
			new Rebuild(),
			new Vagrant(),
			new Baron(),
			new Conspirator(),
			new GreatHall(),
			new Nobles(),
			new WishingWell()
		};

		public Prophecy()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Prophecy"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Intrigue; }
		}
	}

	public sealed class Invasion : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Beggar(),
			new Marauder(),
			new Rogue(),
			new Squire(),
			new Urchin(),
			new Harem(),
			new MiningVillage(),
			new Swindler(),
			new Torturer(),
			new Upgrade()
		};

		public Invasion()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Invasion"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Intrigue; }
		}
	}

	public sealed class WateryGraves : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Count(),
			new Graverobber(),
			new Hermit(),
			new Scavenger(),
			new Urchin(),
			new NativeVillage(),
			new PirateShip(),
			new Salvager(),
			new TreasureMap(),
			new Treasury()
		};

		public WateryGraves()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Watery Graves"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Seaside; }
		}
	}

	public sealed class Peasants : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new DeathCart(),
			new Feodum(),
			new PoorHouse(),
			new Urchin(),
			new Vagrant(),
			new FishingVillage(),
			new Haven(),
			new Island(),
			new Lookout(),
			new Warehouse()
		};

		public Peasants()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Peasants"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Seaside; }
		}
	}

	public sealed class Infestations : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Armory(),
			new Cultist(),
			new Feodum(),
			new MarketSquare(),
			new Rats(),
			new WanderingMinstrel(),
			new Apprentice(),
			new ScryingPool(),
			new Transmute(),
			new Vineyard()
		};

		public Infestations()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Infestations"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Alchemy; }
		}
	}

	public sealed class Lamentations : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Beggar(),
			new Catacombs(),
			new Counterfeit(),
			new Forager(),
			new Ironmonger(),
			new Pillage(),
			new Apothecary(),
			new Golem(),
			new Herbalist(),
			new University()
		};

		public Lamentations()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Lamentations"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Alchemy; }
		}
	}

	public sealed class OneMansTrash : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Counterfeit(),
			new Forager(),
			new Graverobber(),
			new MarketSquare(),
			new Rogue(),
			new City(),
			new GrandMarket(),
			new Monument(),
			new Talisman(),
			new Venture()
		};

		public OneMansTrash()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "One Man's Trash"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Prosperity; }
		}
	}

	public sealed class HonorAmongThieves : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new BanditCamp(),
			new Procession(),
			new Rebuild(),
			new Rogue(),
			new Squire(),
			new Forge(),
			new Hoard(),
			new Peddler(),
			new Quarry(),
			new Watchtower()
		};

		public HonorAmongThieves()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Honor Among Thieves"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Prosperity; }
		}
	}

	public sealed class DarkCarnival : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new BandOfMisfits(),
			new Cultist(),
			new Fortress(),
			new Hermit(),
			new JunkDealer(),
			new Knights(),
			new Fairgrounds(),
			new Hamlet(),
			new HornOfPlenty(),
			new Menagerie()
		};

		public DarkCarnival()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Dark Carnival"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Cornucopia; }
		}
	}

	public sealed class ToTheVictor : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new BanditCamp(),
			new Counterfeit(),
			new DeathCart(),
			new Marauder(),
			new Pillage(),
			new Sage(),
			new Harvest(),
			new HuntingParty(),
			new Remake(),
			new Tournament()
		};

		public ToTheVictor()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "To The Victor"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Cornucopia; }
		}
	}

	public sealed class FarFromHome : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Beggar(),
			new Count(),
			new Feodum(),
			new Marauder(),
			new WanderingMinstrel(),
			new Cartographer(),
			new Develop(),
			new Embassy(),
			new FoolsGold(),
			new Haggler()
		};

		public FarFromHome()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Far From Home"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Hinterlands; }
		}
	}

	public sealed class Expeditions : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Altar(),
			new Catacombs(),
			new Ironmonger(),
			new PoorHouse(),
			new Storeroom(),
			new Crossroads(),
			new Farmland(),
			new Highway(),
			new SpiceMerchant(),
			new Tunnel()
		};

		public Expeditions()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Expeditions"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges | GameSets.Hinterlands; }
		}
	}
}
