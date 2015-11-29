using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Navigator : SeasideCardModel
	{
		public Navigator()
		{
			this.Type = CardType.Action;
			this.Name = "Navigator";
			this.Cost = 4;
			this.Coins = 2;
		}

		private static string[] choices = new string[] { "Keep", "Discard" };

		public override void Play(GameModel gameModel)
		{
			List<CardModel> cards = gameModel.CurrentPlayer.DrawCards(5);
			if (cards.Count > 0)
			{
				gameModel.CurrentPlayer.RevealCards(cards);

				int lookAtCards = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardOrPutOnDeck, cards, "Discard them?", choices, choices);
				if (lookAtCards == 0)
				{
					IEnumerable<CardModel> order = gameModel.CurrentPlayer.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put them back in any order(first on top)", cards);
					foreach (CardModel card in order.Reverse())
					{
						gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
					}
				}
				else
				{
					foreach (CardModel card in cards)
					{
						gameModel.CurrentPlayer.DiscardCard(card);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Navigator();
		}
	}
}