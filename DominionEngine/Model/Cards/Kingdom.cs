using Dominion.Model.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
	public enum StartingHandType
	{
		Random, //every player's hand is determined randomly
		RandomSameStartingHands, // the distribution is determined randomly but is the same for each player
		FourThreeSplit, // all players have a 4/3 split
		FiveTwoSplit // all players have a 5/2 split
	}

	public enum CardUseType
	{
		Random, //50% chance
		RandomByCardsFromSet, //10% chance per kingdom card in set
		Use,
		DoNotUse
	}

	public class Kingdom
	{
		public Kingdom(IList<CardModel> cards, IList<CardModel> prohibitedCards, GameSets allowedSets, int numPlayers)
			: this(cards, prohibitedCards, null, allowedSets, numPlayers, CardUseType.RandomByCardsFromSet, CardUseType.RandomByCardsFromSet, StartingHandType.Random, false)
		{
		}

		public Kingdom(IList<CardModel> cards, IList<CardModel> prohibitedCards, CardModel bane, GameSets allowedSets, int numPlayers)
			: this(cards, prohibitedCards, bane, allowedSets, numPlayers, CardUseType.RandomByCardsFromSet, CardUseType.RandomByCardsFromSet, StartingHandType.Random, false)
		{
		}

		public Kingdom(IList<CardModel> cards, IList<CardModel> prohibitedCards, CardModel bane, GameSets allowedSets, int numPlayers, CardUseType usesColonies, CardUseType usesShelters, StartingHandType startingHandType)
			: this(cards, prohibitedCards, bane, allowedSets, numPlayers, usesColonies, usesShelters, startingHandType, false)
		{
		}

		public Kingdom(IList<CardModel> cards, IList<CardModel> prohibitedCards, CardModel bane, GameSets allowedSets, int numPlayers, CardUseType usesColonies, CardUseType usesShelters, StartingHandType startingHandType, bool useRandomCardsFromChosenSetsOnly)
		{
			this.Bane = bane;
			this.NumPlayers = numPlayers;
			this.AllowedSets = allowedSets;
			if (prohibitedCards == null)
			{
				prohibitedCards = new List<CardModel>();
			}
			List<CardModel> newCards = new List<CardModel>();
			if (cards != null)
			{
				newCards.AddRange(cards);
			}
			this.Cards = newCards;

			int need = 10 - this.Cards.Count;
			if (need > 0)
			{				
				IEnumerable<CardModel> randomFilteredCards = RandomAllCardSet.RandomCardIDs.Where(c => !this.Cards.Any(cc => cc.Name == c.Name) && !prohibitedCards.Any(cc => cc.Name == c.Name) && (this.Bane == null || this.Bane.Name != c.Name) && (c.GameSet & this.AllowedSets) != 0).OrderBy(c => Randomizer.Next());
				newCards.AddRange(randomFilteredCards.Take(need));
				need = 10 - this.Cards.Count;
				if (need > 0)
				{
					IEnumerable<CardModel> randomAllCards = RandomAllCardSet.RandomCardIDs.Where(c => !this.Cards.Any(cc => cc.Name == c.Name) && (this.Bane == null || this.Bane.Name != c.Name)).OrderBy(c => Randomizer.Next());
					newCards.AddRange(randomAllCards.Take(need));
				}
			}

			this.InitializeBlackMarket(useRandomCardsFromChosenSetsOnly, prohibitedCards);
			if (this.Cards.Any(card => card is YoungWitch))
			{
				if (this.Bane == null)
				{
					// add a bane card if it hasn't been set already
					IEnumerable<CardModel> baneCards = RandomAllCardSet.RandomCardIDs.Where(c => (c.GetBaseCost() == 2 || c.GetBaseCost() == 3) && !c.CostsPotion && !this.Cards.Any(cc => cc.Name == c.Name) && (this.BlackMarketDeck == null || !this.BlackMarketDeck.Any(cc => cc.Name == c.Name)) && (c.GameSet & this.AllowedSets) != 0);
					if (useRandomCardsFromChosenSetsOnly)
					{
						IEnumerable<CardModel> filteredBaneCards = baneCards.Where(c => this.Cards.Any(cc => c.GameSet == cc.GameSet) && !prohibitedCards.Any(cc => cc.Name == c.Name));
						if(filteredBaneCards.Any())
						{
							baneCards = filteredBaneCards;
						}
					}
					
					CardModel baneChoice = baneCards.ElementAt(Randomizer.Next(baneCards.Count()));
					this.Bane = baneChoice;
				}
				if (this.Bane is BlackMarket)
				{
					this.InitializeBlackMarket(useRandomCardsFromChosenSetsOnly, prohibitedCards);
				}
				this.Cards.Add(this.Bane);
			}

			newCards.Sort(new Comparison<CardModel>((CardModel lhs, CardModel rhs) =>
			{
				if (lhs.CostsPotion && !rhs.CostsPotion) return 1;
				if (rhs.CostsPotion && !lhs.CostsPotion) return -1;
				if (lhs.GetBaseCost() != rhs.GetBaseCost()) return lhs.GetBaseCost() - rhs.GetBaseCost();
				return lhs.Name.CompareTo(rhs.Name);
			}));

			switch (usesColonies)
			{
				case CardUseType.Random:
					this.UsesColonies = Randomizer.Next(2) == 1;
					break;

				case CardUseType.RandomByCardsFromSet:
					this.UsesColonies = this.Cards.Count(card => card is ProsperityCardModel) > Randomizer.Next(10);
					break;

				case CardUseType.Use:
					this.UsesColonies = true;
					break;

				case CardUseType.DoNotUse:
					this.UsesColonies = false;
					break;
			}

			switch (usesShelters)
			{
				case CardUseType.Random:
					this.UsesShelters = Randomizer.Next(2) == 1;
					break;

				case CardUseType.RandomByCardsFromSet:
					this.UsesShelters = this.Cards.Count(card => card is DarkAgesCardModel) > Randomizer.Next(10);
					break;

				case CardUseType.Use:
					this.UsesShelters = true;
					break;

				case CardUseType.DoNotUse:
					this.UsesShelters = false;
					break;
			}

			this.CreateStartingDecks(startingHandType);

			this.VictoryCardCount = this.NumPlayers < 3 ? 8 : 12;
		}

		private void CreateStartingDecks(StartingHandType startingHandType)
		{
			this.StartingDecks = new List<List<CardModel>>();
			for (int i = 0; i < this.NumPlayers; i++)
			{
				List<CardModel> deck = new List<CardModel>();
				this.StartingDecks.Add(deck);
				List<CardModel> coppers = new List<CardModel>();
				List<CardModel> nonCoppers = new List<CardModel>();
				if (this.UsesShelters)
				{
					nonCoppers.Add(new Hovel());
					nonCoppers.Add(new Necropolis());
					nonCoppers.Add(new OvergrownEstate());
					nonCoppers.Shuffle();
				}
				else
				{
					for (int j = 0; j < 3; j++)
					{
						nonCoppers.Add(new Estate());
					}
				}
				for (int j = 0; j < 7; j++)
				{
					coppers.Add(new Copper());
				}
				
				if (startingHandType == StartingHandType.FourThreeSplit)
				{
					deck.AddRange(coppers.Take(4));
					deck.AddRange(nonCoppers.Take(1));
					deck.AddRange(coppers.Skip(4));
					deck.AddRange(nonCoppers.Skip(1));
				}
				else if(startingHandType == StartingHandType.FiveTwoSplit)
				{
					deck.AddRange(coppers);
					deck.AddRange(nonCoppers);
				}
				else if(startingHandType == StartingHandType.Random)
				{
					deck.AddRange(coppers);
					deck.AddRange(nonCoppers);
					deck.Shuffle();
				}
				else
				{
					deck.AddRange(coppers);
					deck.AddRange(nonCoppers);
					deck.Shuffle();
					int count = deck.Take(5).Count(c => c is Copper);
					if(count == 2 || count == 5)
					{
						startingHandType = StartingHandType.FiveTwoSplit;
					}
					else
					{
						startingHandType = StartingHandType.FourThreeSplit;
					}
				}
			}
		}

		private void InitializeBlackMarket(bool useRandomCardsFromChosenSetsOnly, IList<CardModel> prohibitedCards)
		{
			if ((this.Cards.Any(card => card is BlackMarket) || this.Bane is BlackMarket) && this.BlackMarketDeck == null)
			{
				IEnumerable<CardModel> blackMarket = RandomAllCardSet.RandomCardIDs.Where(card => !this.Cards.Any(pile => pile.GetType() == card.GetType()) && (this.Bane == null || this.Bane.Name != card.Name) && !card.Is(CardType.Knight) && (card.GameSet & this.AllowedSets) != 0 && !prohibitedCards.Any(cc => cc.Name == card.Name)).ToList();
				if (!this.Cards.Any(c => c.Is(CardType.Knight)) && (this.AllowedSets & GameSets.DarkAges) != 0)
				{
					// Maximum 1 knight allowed in black market.
					blackMarket = blackMarket.Union(Knights.AllKnights.OrderBy(c => Randomizer.Next()).Take(1));
				}
				if (useRandomCardsFromChosenSetsOnly)
				{
					IEnumerable<CardModel> filteredBlackMarket = blackMarket.Where(card => this.Cards.Any(cc => card.GameSet == cc.GameSet));
					if(filteredBlackMarket.Count() >= 15)
					{
						blackMarket = filteredBlackMarket.ToList();
					}
				}				

				blackMarket = blackMarket.OrderBy(c => Randomizer.Next()).Take(15);
				this.BlackMarketDeck = blackMarket.ToList();
			}
		}
		public int NumPlayers { get; private set; }
		public bool UsesShelters { get; private set; }
		public bool UsesColonies { get; private set; }

		// Includes Bane
		public IList<CardModel> Cards { get; private set; }
		public CardModel Bane { get; private set; }
		public IList<CardModel> BlackMarketDeck { get; private set; }

		public IList<List<CardModel>> StartingDecks { get; private set; }

		public GameSets AllowedSets { get; private set; }

		public int VictoryCardCount { get; private set; }
	}
}
