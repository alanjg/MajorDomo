using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class JackOfAllTrades : HinterlandsCardModel
	{
		public JackOfAllTrades()
		{
			this.Name = "Jack of All Trades";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		private static string[] trashChoices = new string[] { "Yes", "No" };
		private static string[] inspectChoices = new string[] { "Keep", "Discard" };
		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Silver));
			CardModel top = gameModel.CurrentPlayer.DrawCard();
			if (top != null)
			{
				EffectChoiceType choiceType = gameModel.CurrentPlayer.Hand.Count < 5 ? EffectChoiceType.DiscardOrPutOnDeckToDraw : EffectChoiceType.DiscardOrPutOnDeck;
				int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(choiceType, top, "You revealed a " + top.Name, inspectChoices, inspectChoices);
				if (choice == 0)
				{
					gameModel.CurrentPlayer.Deck.PlaceOnTop(top);
				}
				else
				{
					gameModel.CurrentPlayer.DiscardCard(top);
				}
			}
			gameModel.CurrentPlayer.DrawTo(5);
			if (gameModel.CurrentPlayer.Hand.Count > 0)
			{
				IEnumerable<CardModel> trashable = gameModel.CurrentPlayer.Hand.Where(c => !c.Is(CardType.Treasure));
				if (trashable.Any())
				{
					CardModel toTrash = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashFromHand, "You may trash a card from hand", ChoiceSource.FromHand, trashable);
					if (toTrash != null)
					{
						gameModel.CurrentPlayer.Trash(toTrash);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new JackOfAllTrades();
		}
	}
}
