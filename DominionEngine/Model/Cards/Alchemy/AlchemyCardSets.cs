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
	public sealed class ForbiddenArts : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Apprentice(),
			new Familiar(),
			new Possession(),
			new University(),
			new Cellar(),
			new CouncilRoom(),
			new Gardens(),
			new Laboratory(),
			new Thief(),
			new ThroneRoom(),
		};

		public ForbiddenArts()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Forbidden Arts"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Alchemy | GameSets.Base; }
		}
	}

	public sealed class PotionMixers : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Alchemist(),
			new Apothecary(),
			new Golem(),
			new Herbalist(),
			new Transmute(),
			new Cellar(),
			new Chancellor(),
			new Festival(),
			new Militia(),
			new Smithy(),
		};

		public PotionMixers()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Potion Mixers"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Alchemy | GameSets.Base; }
		}
	}

	public sealed class ChemistryLesson : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Alchemist(),
			new Golem(),
			new PhilosophersStone(),
			new University(),
			new Bureaucrat(),
			new Market(),
			new Moat(),
			new Remodel(),
			new Witch(),
			new Woodcutter(),
		};

		public ChemistryLesson()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Chemistry Lesson"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Alchemy | GameSets.Base; }
		}
	}

	public sealed class Servants : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Golem(),
			new Possession(),
			new ScryingPool(),
			new Transmute(),
			new Vineyard(),
			new Conspirator(),
			new GreatHall(),
			new Minion(),
			new Pawn(),
			new Steward(),
		};

		public Servants()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Servants"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Alchemy | GameSets.Intrigue; }
		}
	}

	public sealed class SecretResearch : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Familiar(),
			new Herbalist(),
			new PhilosophersStone(),
			new University(),
			new Bridge(),
			new Masquerade(),
			new Minion(),
			new Nobles(),
			new ShantyTown(),
			new Torturer(),
		};

		public SecretResearch()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Secret Research"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Alchemy | GameSets.Intrigue; }
		}
	}

	public sealed class PoolsToolsAndFools : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Apothecary(),
			new Apprentice(),
			new Golem(),
			new ScryingPool(),
			new Baron(),
			new Coppersmith(),
			new Ironworks(),
			new Nobles(),
			new TradingPost(),
			new WishingWell(),
		};

		public PoolsToolsAndFools()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Pools, Tools, and Fools"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Alchemy | GameSets.Intrigue; }
		}
	}
}