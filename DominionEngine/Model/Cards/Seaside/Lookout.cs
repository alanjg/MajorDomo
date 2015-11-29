using System;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Lookout : SeasideCardModel
	{
		public Lookout()
		{
			this.Type = CardType.Action;
			this.Name = "Lookout";
			this.Cost = 3;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			Player player = gameModel.CurrentPlayer;
			List<CardModel> cards = player.DrawCards(3);

			if (cards.Count > 0)
			{
				CardModel trash = player.Chooser.ChooseOneCard(CardChoiceType.Trash, "Trash a card", Chooser.ChoiceSource.None, cards);
				player.Trash(trash);
				cards.Remove(trash);

				if (cards.Count > 0)
				{
					CardModel discard = player.Chooser.ChooseOneCard(CardChoiceType.Discard, "Discard a card", Chooser.ChoiceSource.None, cards);
					player.DiscardCard(discard);
					cards.Remove(discard);

					if (cards.Count > 0)
					{
						player.Deck.PlaceOnTop(cards[0]);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Lookout();
		}
	}
}