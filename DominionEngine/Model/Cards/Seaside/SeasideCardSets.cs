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
	public sealed class HighSeas : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Bazaar(),
			new Caravan(),
			new Embargo(),
			new Explorer(),
			new Haven(),
			new Island(),
			new Lookout(),
			new PirateShip(),
			new Smugglers(),
			new Wharf()
		};

		public HighSeas()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "High Seas"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Seaside; }
		}
	}

	public sealed class BuriedTreasure : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Ambassador(),
			new Cutpurse(),
			new FishingVillage(),
			new Lighthouse(),
			new Outpost(),
			new PearlDiver(),
			new Tactician(),
			new TreasureMap(),
			new Warehouse(),
			new Wharf()
		};

		public BuriedTreasure()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Buried Treasure"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Seaside; }
		}
	}

	public sealed class Shipwrecks : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new GhostShip(),
			new MerchantShip(),
			new NativeVillage(),
			new Navigator(),
			new PearlDiver(),
			new Salvager(),
			new SeaHag(),
			new Smugglers(),
			new Treasury(),
			new Warehouse(),
		};

		public Shipwrecks()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Shipwrecks"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Seaside; }
		}
	}

	public sealed class ReachForTomorrow : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Adventurer(),
			new Cellar(),
			new CouncilRoom(),
			new Cutpurse(),
			new GhostShip(),
			new Lookout(),
			new SeaHag(),
			new Spy(),
			new TreasureMap(),
			new Village()
		};

		public ReachForTomorrow()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Reach for Tomorrow"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Seaside | GameSets.Base; }
		}
	}

	public sealed class Repetition : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Caravan(),
			new Chancellor(),
			new Explorer(),
			new Festival(),
			new Militia(),
			new Outpost(),
			new PearlDiver(),
			new PirateShip(),
			new Treasury(),
			new Workshop(),
		};

		public Repetition()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Repetition"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Seaside | GameSets.Base; }
		}
	}

	public sealed class GiveAndTake : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Ambassador(),
			new FishingVillage(),
			new Haven(),
			new Island(),
			new Library(),
			new Market(),
			new Moneylender(),
			new Salvager(),
			new Smugglers(),
			new Witch()
		};

		public GiveAndTake()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Give and Take"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Seaside | GameSets.Base; }
		}
	}
	
	public sealed class RandomSeaside : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Ambassador(),
			new Bazaar(),
			new Caravan(),
			new Cutpurse(),
			new Embargo(),
			new Explorer(),
			new FishingVillage(),
			new GhostShip(),
			new Haven(),
			new Island(),
			new Lighthouse(),
			new Lookout(),
			new MerchantShip(),
			new NativeVillage(),
			new Navigator(),
			new Outpost(),
			new PearlDiver(),
			new PirateShip(),
			new Salvager(),
			new SeaHag(),
			new Smugglers(),
			new Tactician(),
			new TreasureMap(),
			new Treasury(),
			new Warehouse(),
			new Wharf()
		};

		public RandomSeaside()
		{
			this.cardCollection.AddRange(cardIDs.OrderBy(card => Randomizer.Next()).Take(10));
		}

		public override string CardSetName
		{
			get { return "Random (Seaside)"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Seaside; }
		}
	}
}