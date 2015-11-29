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
	public sealed class ArtsAndCrafts : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Stonemason(),
			new Advisor(),
			new Baker(),
			new Journeyman(),
			new MerchantGuild(),
			new Laboratory(),
			new Cellar(),
			new Workshop(),
			new Festival(),
			new Moneylender()
		};

		public ArtsAndCrafts()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Arts And Crafts"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Guilds | GameSets.Base; }
		}
	}

	public sealed class CleanLiving : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Butcher(),
			new Baker(),
			new CandlestickMaker(),
			new Doctor(),
			new Soothsayer(),
			new Militia(),
			new Thief(),
			new Moneylender(),
			new Gardens(),
			new Village()
		};

		public CleanLiving()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Clean Living"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Guilds | GameSets.Base; }
		}
	}

	public sealed class GildingTheLily : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Plaza(),
			new Masterpiece(),
			new CandlestickMaker(),
			new Taxman(),
			new Herald(),
			new Library(),
			new Remodel(),
			new Adventurer(),
			new Market(),
			new Chancellor()
		};

		public GildingTheLily()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Gilding the Lily"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Guilds | GameSets.Base; }
		}
	}

	public sealed class NameThatCard : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Baker(),
			new Doctor(),
			new Plaza(),
			new Advisor(),
			new Masterpiece(),
			new Courtyard(),
			new WishingWell(),
			new Harem(),
			new Tribute(),
			new Nobles()
		};

		public NameThatCard()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Name That Card"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Guilds | GameSets.Intrigue; }
		}
	}

	public sealed class TricksOfTheTrade : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Stonemason(),
			new Herald(),
			new Soothsayer(),
			new Journeyman(),
			new Butcher(),
			new GreatHall(),
			new Nobles(),
			new Conspirator(),
			new Masquerade(),
			new Coppersmith()
		};

		public TricksOfTheTrade()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Tricks of the Trade"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Guilds | GameSets.Intrigue; }
		}
	}

	public sealed class DecisionsDecisions : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new MerchantGuild(),
			new CandlestickMaker(),
			new Masterpiece(),
			new Taxman(),
			new Butcher(),
			new Bridge(),
			new Pawn(),
			new MiningVillage(),
			new Upgrade(),
			new Duke()
		};

		public DecisionsDecisions()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Decisions, Decisions"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Guilds | GameSets.Intrigue; }
		}
	}
}
