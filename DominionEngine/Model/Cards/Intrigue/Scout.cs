using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Scout : IntrigueCardModel
	{
		public Scout()
		{
			this.Type = CardType.Action;
			this.Name = "Scout";
			this.Cost = 4;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			List<CardModel> topCards = currentPlayer.DrawCards(4);
			if (topCards.Count > 0)
			{
				currentPlayer.RevealCards(topCards);

				foreach (CardModel victoryCard in topCards.Where(c => c.Is(CardType.Victory)))
				{
					currentPlayer.PutInHand(victoryCard);
				}

				IEnumerable<CardModel> otherCards = topCards.Where(c => !c.Is(CardType.Victory));
				IEnumerable<CardModel> choices = currentPlayer.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put the remaining cards back in any order(first on top)", otherCards);
				foreach (CardModel card in choices.Reverse())
				{
					currentPlayer.Deck.PlaceOnTop(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Scout();
		}
	}
}
