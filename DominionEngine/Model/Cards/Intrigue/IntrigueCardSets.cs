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
	public static class IntrigueCardSets
	{
		public static readonly CardSet VictoryDance = new VictoryDance();
		public static readonly CardSet SecretSchemes = new SecretSchemes();
		public static readonly CardSet Deconstruction = new Deconstruction();
	}

	public sealed class VictoryDance : CardSet
	{
		public VictoryDance()
		{
			this.cardCollection.Add(new Bridge());
			this.cardCollection.Add(new Duke());
			this.cardCollection.Add(new GreatHall());
			this.cardCollection.Add(new Harem());
			this.cardCollection.Add(new Ironworks());
			this.cardCollection.Add(new Masquerade());
			this.cardCollection.Add(new Nobles());
			this.cardCollection.Add(new Pawn());
			this.cardCollection.Add(new Scout());
			this.cardCollection.Add(new Upgrade());
		}

		public override string CardSetName
		{
			get { return "Victory Dance"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Intrigue; }
		}
	}

	public sealed class SecretSchemes : CardSet
	{
		public SecretSchemes()
		{
			this.cardCollection.Add(new Conspirator());
			this.cardCollection.Add(new Harem());
			this.cardCollection.Add(new Ironworks());
			this.cardCollection.Add(new Pawn());
			this.cardCollection.Add(new Saboteur());
			this.cardCollection.Add(new ShantyTown());
			this.cardCollection.Add(new Steward());
			this.cardCollection.Add(new Swindler());
			this.cardCollection.Add(new TradingPost());
			this.cardCollection.Add(new Tribute());
		}

		public override string CardSetName
		{
			get { return "Secret Schemes"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Intrigue; }
		}
	}

	public sealed class BestWishes : CardSet
	{
		public BestWishes()
		{
			this.cardCollection.Add(new Coppersmith());
			this.cardCollection.Add(new Courtyard());
			this.cardCollection.Add(new Masquerade());
			this.cardCollection.Add(new Scout());
			this.cardCollection.Add(new ShantyTown());
			this.cardCollection.Add(new Steward());
			this.cardCollection.Add(new Torturer());
			this.cardCollection.Add(new TradingPost());
			this.cardCollection.Add(new Upgrade());
			this.cardCollection.Add(new WishingWell());
		}

		public override string CardSetName
		{
			get { return "Best Wishes"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Intrigue; }
		}
	}

	public sealed class Deconstruction : CardSet
	{
		public Deconstruction()
		{
			this.cardCollection.Add(new Bridge());
			this.cardCollection.Add(new MiningVillage());
			this.cardCollection.Add(new Remodel());
			this.cardCollection.Add(new Saboteur());
			this.cardCollection.Add(new SecretChamber());
			this.cardCollection.Add(new Spy());
			this.cardCollection.Add(new Swindler());
			this.cardCollection.Add(new Thief());
			this.cardCollection.Add(new ThroneRoom());
			this.cardCollection.Add(new Torturer());
		}

		public override string CardSetName
		{
			get { return "Deconstruction"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base | GameSets.Intrigue; }
		}
	}

	public sealed class HandMadness : CardSet
	{
		public HandMadness()
		{
			this.cardCollection.Add(new Bureaucrat());
			this.cardCollection.Add(new Chancellor());
			this.cardCollection.Add(new CouncilRoom());
			this.cardCollection.Add(new Courtyard());
			this.cardCollection.Add(new Mine());
			this.cardCollection.Add(new Militia());
			this.cardCollection.Add(new Minion());
			this.cardCollection.Add(new Nobles());
			this.cardCollection.Add(new Steward());
			this.cardCollection.Add(new Torturer());
		}

		public override string CardSetName
		{
			get { return "Hand Madness"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base | GameSets.Intrigue; }
		}
	}

	public sealed class Underlings : CardSet
	{
		public Underlings()
		{
			this.cardCollection.Add(new Baron());
			this.cardCollection.Add(new Cellar());
			this.cardCollection.Add(new Festival());
			this.cardCollection.Add(new Library());
			this.cardCollection.Add(new Masquerade());
			this.cardCollection.Add(new Minion());
			this.cardCollection.Add(new Nobles());
			this.cardCollection.Add(new Pawn());
			this.cardCollection.Add(new Steward());
			this.cardCollection.Add(new Witch());
		}

		public override string CardSetName
		{
			get { return "Underlings"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base | GameSets.Intrigue; }
		}
	}

	public sealed class RandomIntrigue : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Baron(),
			new Bridge(),
			new Conspirator(),
			new Coppersmith(),
			new Courtyard(),
			new Duke(),
			new GreatHall(),
			new Harem(),
			new Ironworks(),
			new Masquerade(),
			new MiningVillage(),
			new Minion(),
			new Nobles(),
			new Pawn(),
			new Saboteur(),
			new Scout(),
			new SecretChamber(),
			new ShantyTown(),
			new Steward(),
			new Swindler(),
			new Torturer(),
			new TradingPost(),
			new Tribute(),
			new Upgrade(),
			new WishingWell()
		};

		public RandomIntrigue()
		{
			this.cardCollection.AddRange(cardIDs.OrderBy(card => Randomizer.Next()).Take(10));
		}

		public override string CardSetName
		{
			get { return "Random (Intrigue)"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Intrigue; }
		}
	}
}