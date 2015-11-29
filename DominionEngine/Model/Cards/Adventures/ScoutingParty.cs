using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class ScoutingParty : AdventuresCardModel
	{
		public ScoutingParty()
		{
			this.Name = "Scouting Party";
			this.Type = CardType.Event;
			this.Cost = 2;
			
			this.Buys = 1;
		}

		public override void OnBuy(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			List<CardModel> topCards = currentPlayer.DrawCards(5);
			int discardCount = Math.Min(3, topCards.Count());
			IEnumerable<CardModel> choices = currentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.DiscardOrPutOnDeck, "Discard three of them", ChoiceSource.None, discardCount, discardCount, topCards);
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
		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}

		public override CardModel Clone()
		{
			return new ScoutingParty();
		}
	}
}
