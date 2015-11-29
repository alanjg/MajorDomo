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
	public sealed class BasicGame : CardSet
	{
		public BasicGame()
		{
			this.cardCollection.Add(new Cellar());
			this.cardCollection.Add(new Market());
			this.cardCollection.Add(new Militia());
			this.cardCollection.Add(new Mine());
			this.cardCollection.Add(new Moat());
			this.cardCollection.Add(new Remodel());
			this.cardCollection.Add(new Smithy());
			this.cardCollection.Add(new Village());
			this.cardCollection.Add(new Woodcutter());
			this.cardCollection.Add(new Workshop());
		}

		public override string CardSetName
		{
			get { return "Basic Game"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base; }
		}
	}

	public sealed class BigMoney : CardSet
	{
		public BigMoney()
		{
			this.cardCollection.Add(new Adventurer());
			this.cardCollection.Add(new Bureaucrat());
			this.cardCollection.Add(new Chancellor());
			this.cardCollection.Add(new Chapel());
			this.cardCollection.Add(new Feast());
			this.cardCollection.Add(new Laboratory());
			this.cardCollection.Add(new Market());
			this.cardCollection.Add(new Mine());
			this.cardCollection.Add(new Moneylender());
			this.cardCollection.Add(new ThroneRoom());
		}

		public override string CardSetName
		{
			get { return "Big Money"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base; }
		}
	}

	public sealed class Interaction : CardSet
	{
		public Interaction()
		{
			this.cardCollection.Add(new Bureaucrat());
			this.cardCollection.Add(new Chancellor());
			this.cardCollection.Add(new CouncilRoom());
			this.cardCollection.Add(new Festival());
			this.cardCollection.Add(new Library());
			this.cardCollection.Add(new Militia());
			this.cardCollection.Add(new Moat());
			this.cardCollection.Add(new Spy());
			this.cardCollection.Add(new Thief());
			this.cardCollection.Add(new Village());
		}

		public override string CardSetName
		{
			get { return "Interaction"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base; }
		}
	}

	public sealed class SizeDistortion : CardSet
	{
		public SizeDistortion()
		{
			this.cardCollection.Add(new Cellar());
			this.cardCollection.Add(new Chapel());
			this.cardCollection.Add(new Feast());
			this.cardCollection.Add(new Gardens());
			this.cardCollection.Add(new Laboratory());
			this.cardCollection.Add(new Thief());
			this.cardCollection.Add(new Village());
			this.cardCollection.Add(new Witch());
			this.cardCollection.Add(new Woodcutter());
			this.cardCollection.Add(new Workshop());
		}

		public override string CardSetName
		{
			get { return "Size Distortion"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base; }
		}
	}

	public sealed class VillageSquare : CardSet
	{
		public VillageSquare()
		{
			this.cardCollection.Add(new Bureaucrat());
			this.cardCollection.Add(new Cellar());
			this.cardCollection.Add(new Festival());
			this.cardCollection.Add(new Library());
			this.cardCollection.Add(new Market());
			this.cardCollection.Add(new Remodel());
			this.cardCollection.Add(new Smithy());
			this.cardCollection.Add(new ThroneRoom());
			this.cardCollection.Add(new Village());
			this.cardCollection.Add(new Woodcutter());
		}

		public override string CardSetName
		{
			get { return "Village Square"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base; }
		}
	}

	public sealed class RandomBase : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Adventurer(),
			new Bureaucrat(),
			new Cellar(),
			new Chancellor(),
			new Chapel(),
			new CouncilRoom(),
			new Feast(),
			new Festival(),
			new Gardens(),
			new Laboratory(),
			new Library(),
			new Market(),
			new Militia(),
			new Mine(),
			new Moat(),
			new Moneylender(),
			new Remodel(),
			new Smithy(),
			new Spy(),
			new Thief(),
			new ThroneRoom(),
			new Village(),
			new Witch(),
			new Woodcutter(),
			new Workshop(),
		};

		public RandomBase()
		{
			this.cardCollection.AddRange(cardIDs.OrderBy(card => Randomizer.Next()).Take(10));
		}

		public override string CardSetName
		{
			get { return "Random (Base Set)"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base; }
		}
	}
}