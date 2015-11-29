using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Cartographer : HinterlandsCardModel
	{
		public Cartographer()
		{
			this.Name = "Cartographer";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Cards = 1;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			List<CardModel> topCards = currentPlayer.DrawCards(4);

			IEnumerable<CardModel> choices = currentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.DiscardOrPutOnDeck, "Discard any", ChoiceSource.None, 0, topCards.Count(), topCards);
			foreach (CardModel card in choices)
			{
				currentPlayer.DiscardCard(card);
				topCards.Remove(card);
			}

			IEnumerable<CardModel> order = currentPlayer.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put the rest back in any order(first on top)", topCards);
			foreach (CardModel card in order.Reverse().ToList())
			{
				currentPlayer.Deck.PlaceOnTop(card);
			}
		}

		public override CardModel Clone()
		{
			return new Cartographer();
		}
	}
}
