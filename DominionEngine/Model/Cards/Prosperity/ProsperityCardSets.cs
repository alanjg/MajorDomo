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
	public sealed class Beginners : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Bank(),
			new CountingHouse(),
			new Expand(),
			new Goons(),
			new Monument(),
			new Rabble(),
			new RoyalSeal(),
			new Venture(),
			new Watchtower(),
			new WorkersVillage(),
		};

		public Beginners()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Beginners"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Prosperity; }
		}
	}

	public sealed class FriendlyInteractive : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Bishop(),
			new City(),
			new Contraband(),
			new Forge(),
			new Hoard(),
			new Peddler(),
			new RoyalSeal(),
			new TradeRoute(),
			new Vault(),
			new WorkersVillage()
		};

		public FriendlyInteractive()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Friendly Interactive"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Prosperity; }
		}
	}

	public sealed class BigActions : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new City(),
			new Expand(),
			new GrandMarket(),
			new KingsCourt(),
			new Loan(),
			new Mint(),
			new Quarry(),
			new Rabble(),
			new Talisman(),
			new Vault()
		};

		public BigActions()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Big Actions"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Prosperity; }
		}
	}

	public sealed class BiggestMoney : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Bank(),
			new GrandMarket(),
			new Mint(),
			new RoyalSeal(),
			new Venture(),
			new Adventurer(),
			new Laboratory(),
			new Mine(),
			new Moneylender(),
			new Spy()
		};

		public BiggestMoney()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Biggest Money"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base | GameSets.Prosperity; }
		}
	}

	public sealed class TheKingsArmy : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Expand(),
			new Goons(),
			new KingsCourt(),
			new Rabble(),
			new Vault(),
			new Bureaucrat(),
			new CouncilRoom(),
			new Moat(),
			new Spy(),
			new Village()
		};

		public TheKingsArmy()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "The King's Army"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base | GameSets.Prosperity; }
		}
	}

	public sealed class TheGoodLife : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Contraband(),
			new CountingHouse(),
			new Hoard(),
			new Monument(),
			new Mountebank(),
			new Bureaucrat(),
			new Cellar(),
			new Chancellor(),
			new Gardens(),
			new Village()
		};

		public TheGoodLife()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "The Good Life"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base | GameSets.Prosperity; }
		}
	}

	public sealed class PathsToVictory : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Bishop(),
			new CountingHouse(),
			new Goons(),
			new Monument(),
			new Peddler(),
			new Baron(),
			new Harem(),
			new Pawn(),
			new ShantyTown(),
			new Upgrade()
		};

		public PathsToVictory()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Paths To Victory"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Prosperity | GameSets.Intrigue; }
		}
	}

	public sealed class AllAlongTheWatchtower : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Hoard(),
			new Talisman(),
			new TradeRoute(),
			new Vault(),
			new Watchtower(),
			new Bridge(),
			new GreatHall(),
			new MiningVillage(),
			new Pawn(),
			new Torturer()
		};

		public AllAlongTheWatchtower()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "All Along The Watchtower"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Prosperity | GameSets.Intrigue; }
		}
	}

	public sealed class LuckySeven : CardSet
	{
		private static CardModel[] cardIDs = new CardModel[]
		{
			new Bank(),
			new Expand(),
			new Forge(),
			new KingsCourt(),
			new Vault(),
			new Bridge(),
			new Coppersmith(),
			new Swindler(),
			new Tribute(),
			new WishingWell()
		};

		public LuckySeven()
		{
			this.cardCollection.AddRange(cardIDs);
		}

		public override string CardSetName
		{
			get { return "Lucky Seven"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Prosperity | GameSets.Intrigue; }
		}
	}
}