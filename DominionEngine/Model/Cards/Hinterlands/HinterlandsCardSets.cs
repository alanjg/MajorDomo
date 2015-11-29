using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Dominion.Model.PileFactories;
using Dominion.Model.Actions;

namespace Dominion.CardSets
{
	public sealed class Introduction : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Cache(),
			new Crossroads(),
			new Develop(),
			new Haggler(),
			new JackOfAllTrades(),
			new Margrave(),
			new NomadCamp(),
			new Oasis(),
			new SpiceMerchant(),
			new Stables()
		};

		public Introduction()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Introduction"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands; }
		}
	}

	public sealed class FairTrades : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new BorderVillage(),
			new Cartographer(),
			new Develop(),
			new Duchess(),
			new Farmland(),
			new IllGottenGains(),
			new NobleBrigand(),
			new SilkRoad(),
			new Stables(),
			new Trader()
		};

		public FairTrades()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Fair Trades"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands; }
		}
	}

	public sealed class Bargains : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new BorderVillage(),
			new Cache(),
			new Duchess(),
			new FoolsGold(),
			new Haggler(),
			new Highway(),
			new NomadCamp(),
			new Scheme(),
			new SpiceMerchant(),
			new Trader()
		};

		public Bargains()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Bargains"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands; }
		}
	}

	public sealed class Gambits : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Cartographer(),
			new Crossroads(),
			new Embassy(),
			new Inn(),
			new JackOfAllTrades(),
			new Mandarin(),
			new NomadCamp(),
			new Oasis(),
			new Oracle(),
			new Tunnel()
		};

		public Gambits()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Gambits"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands; }
		}
	}

	public sealed class HighwayRobbery : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Cellar(),
			new Library(),
			new Moneylender(),
			new ThroneRoom(),
			new Workshop(),
			new Highway(),
			new Inn(),
			new Margrave(),
			new NobleBrigand(),
			new Oasis()
		};

		public HighwayRobbery()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Highway Robbery"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Base; }
		}
	}

	public sealed class AdventuresAbroad : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Adventurer(),
			new Chancellor(),
			new Festival(),
			new Laboratory(),
			new Remodel(),
			new Crossroads(),
			new Farmland(),
			new FoolsGold(),
			new Oracle(),
			new SpiceMerchant()
		};

		public AdventuresAbroad()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Adventures Abroad"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Base; }
		}
	}

	public sealed class MoneyForNothing : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Coppersmith(),
			new GreatHall(),
			new Pawn(),
			new ShantyTown(),
			new Torturer(),
			new Cache(),
			new Cartographer(),
			new JackOfAllTrades(),
			new SilkRoad(),
			new Tunnel()
		};

		public MoneyForNothing()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Money For Nothing"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Intrigue; }
		}
	}

	public sealed class TheDukesBall : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Conspirator(),
			new Duke(),
			new Harem(),
			new Masquerade(),
			new Upgrade(),
			new Duchess(),
			new Haggler(),
			new Inn(),
			new NobleBrigand(),
			new Scheme()
		};

		public TheDukesBall()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "The Duke's Ball"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Intrigue; }
		}
	}

	public sealed class Travelers : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Cutpurse(),
			new Island(),
			new Lookout(),
			new MerchantShip(),
			new Warehouse(),
			new Cartographer(),
			new Crossroads(),
			new Farmland(),
			new SilkRoad(),
			new Stables()
		};

		public Travelers()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Travelers"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Seaside; }
		}
	}

	public sealed class Diplomacy : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Ambassador(),
			new Bazaar(),
			new Caravan(),
			new Embargo(),
			new Smugglers(),
			new Embassy(),
			new Farmland(),
			new IllGottenGains(),
			new NobleBrigand(),
			new Trader()
		};

		public Diplomacy()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Diplomacy"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Seaside; }
		}
	}

	public sealed class SchemesAndDreams : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Apothecary(),
			new Apprentice(),
			new Herbalist(),
			new PhilosophersStone(),
			new Transmute(),
			new Duchess(),
			new FoolsGold(),
			new IllGottenGains(),
			new JackOfAllTrades(),
			new Scheme()
		};

		public SchemesAndDreams()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Schemes And Dreams"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Alchemy; }
		}
	}

	public sealed class WineCountry : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Apprentice(),
			new Familiar(),
			new Golem(),
			new University(),
			new Vineyard(),
			new Crossroads(),
			new Farmland(),
			new Haggler(),
			new Highway(),
			new NomadCamp(),
		};

		public WineCountry()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Wine Country"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Alchemy; }
		}
	}

	public sealed class InstantGratification : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Bishop(),
			new Expand(),
			new Hoard(),
			new Mint(),
			new Watchtower(),
			new Farmland(),
			new Haggler(),
			new IllGottenGains(),
			new NobleBrigand(),
			new Trader()
		};

		public InstantGratification()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Instant Gratification"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Prosperity; }
		}
	}

	public sealed class TreasureTrove : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Bank(),
			new Monument(),
			new RoyalSeal(),
			new TradeRoute(),
			new Venture(),
			new Cache(),
			new Develop(),
			new FoolsGold(),
			new IllGottenGains(),
			new Mandarin()
		};

		public TreasureTrove()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Treasure Trove"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Prosperity; }
		}
	}

	public sealed class BlueHarvest : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Hamlet(),
			new HornOfPlenty(),
			new HorseTraders(),
			new Jester(),
			new Tournament(),
			new FoolsGold(),
			new Mandarin(),
			new NobleBrigand(),
			new Trader(),
			new Tunnel()
		};

		public BlueHarvest()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Blue Harvest"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Cornucopia; }
		}
	}

	public sealed class TravelingCircus : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Fairgrounds(),
			new FarmingVillage(),
			new HuntingParty(),
			new Jester(),
			new Menagerie(),
			new BorderVillage(),
			new Embassy(),
			new FoolsGold(),
			new NomadCamp(),
			new Oasis()
		};

		public TravelingCircus()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Traveling Circus"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands | GameSets.Cornucopia; }
		}
	}
}