using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class GameSpecificationInfo
	{
		public GameSpecificationInfo()
		{
			this.Cards = new List<string>();
			this.ProhibitedCards = new List<string>();
			this.Players = new List<string>();
			this.UseColonies = CardUseType.RandomByCardsFromSet;
			this.UseShelters = CardUseType.RandomByCardsFromSet;
			this.StartingHandType = Dominion.StartingHandType.RandomSameStartingHands;
		}

		public string InitiatingPlayer { get; set; }
		public List<string> Players { get; set; }
		public StartingHandType StartingHandType { get; set; }
		public CardUseType UseColonies { get; set; }
		public CardUseType UseShelters { get; set; }
		public List<string> Cards { get; set; }
		public List<string> ProhibitedCards { get; set; }
		public string Bane { get; set; }

		public Kingdom ToKingdom()
		{
			List<CardModel> cards = new List<CardModel>();
			foreach(string card in this.Cards)
			{
				cards.Add(CardModelFactory.GetCardModel(card));
			}
			List<CardModel> prohibitedCards = new List<CardModel>();
			foreach (string card in this.ProhibitedCards)
			{
				prohibitedCards.Add(CardModelFactory.GetCardModel(card));
			}
			CardModel bane = this.Bane != null ? CardModelFactory.GetCardModel(this.Bane) : null;
			Kingdom kingdom = new Kingdom(cards, prohibitedCards, bane, GameSets.Any, this.Players.Count, this.UseColonies, this.UseShelters, this.StartingHandType);
			return kingdom;
		}
	}
}