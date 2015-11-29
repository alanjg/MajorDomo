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
	public sealed class BountyOfTheHunt : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Harvest(),
			new HornOfPlenty(),
			new HuntingParty(),
			new Menagerie(),
			new Tournament(),
			new Cellar(),
			new Festival(),
			new Militia(),
			new Moneylender(),
			new Smithy()
		};

		public BountyOfTheHunt()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Bounty Of The Hunt"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Cornucopia | GameSets.Base; }
		}
	}

	public sealed class BadOmens : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new FortuneTeller(),
			new Hamlet(),
			new HornOfPlenty(),
			new Jester(),
			new Remake(),
			new Adventurer(),
			new Bureaucrat(),
			new Laboratory(),
			new Spy(),
			new ThroneRoom()
		};

		public BadOmens()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Bad Omens"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Cornucopia | GameSets.Base; }
		}
	}

	public sealed class TheJestersWorkshop : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Fairgrounds(),
			new FarmingVillage(),
			new HorseTraders(),
			new Jester(),
			new YoungWitch(),
			new Feast(),
			new Laboratory(),
			new Market(),
			new Remodel(),
			new Workshop(),
			new Chancellor()
		};

		public TheJestersWorkshop()
		{
			this.cardCollection.AddRange(cardIDs);
			this.BaneCard = new Chancellor();
		}

		public override string CardSetName
		{
			get { return "The Jester's Workshop"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Cornucopia | GameSets.Base; }
		}
	}

	public sealed class LastLaughs : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new FarmingVillage(),
			new Harvest(),
			new HorseTraders(),
			new HuntingParty(),
			new Jester(),
			new Minion(),
			new Nobles(),
			new Pawn(),
			new Steward(),
			new Swindler()
		};

		public LastLaughs()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Last Laughs"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Cornucopia | GameSets.Intrigue; }
		}
	}

	public sealed class TheSpiceOfLife : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Fairgrounds(),
			new HornOfPlenty(),
			new Remake(),
			new Tournament(),
			new YoungWitch(),
			new Coppersmith(),
			new Courtyard(),
			new GreatHall(),
			new MiningVillage(),
			new Tribute(),
			new WishingWell()
		};

		public TheSpiceOfLife()
		{
			this.cardCollection.AddRange(cardIDs);
			this.BaneCard = new WishingWell();
		}

		public override string CardSetName
		{
			get { return "The Spice Of Life"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Cornucopia | GameSets.Intrigue; }
		}
	}

	public sealed class SmallVictories : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new FortuneTeller(),
			new Hamlet(),
			new HuntingParty(),
			new Remake(),
			new Tournament(),
			new Conspirator(),
			new Duke(),
			new GreatHall(),
			new Harem(),
			new Pawn()
		};

		public SmallVictories()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Small Victories"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Cornucopia | GameSets.Intrigue; }
		}
	}
}