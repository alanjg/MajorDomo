using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Survivors : DarkAgesCardModel
	{
		public Survivors()
		{
			this.Name = "Survivors";
			this.Type = CardType.Action | CardType.Ruins;
			this.Cost = 0;
		}

		private static string[] choices = new string[] { "Keep", "Discard" };
		public override void Play(GameModel gameModel)
		{
			List<CardModel> topCards = gameModel.CurrentPlayer.DrawCards(2);
			string log = gameModel.CurrentPlayer.Name + " reveals " + Log.FormatSortedCards(topCards);
			gameModel.TextLog.WriteLine(log);
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardOrPutOnDeck, topCards, "Discard them, or put them back in any order", choices, choices);
			if (choice == 1)
			{
				foreach (CardModel card in topCards)
				{
					gameModel.CurrentPlayer.DiscardCard(card);
				}
			}
			else
			{
				IEnumerable<CardModel> ordered = gameModel.CurrentPlayer.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put them back in any order(first on top)", topCards);
				foreach (CardModel card in ordered.Reverse())
				{
					gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
				}
			}
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Survivors();
		}
	}
}