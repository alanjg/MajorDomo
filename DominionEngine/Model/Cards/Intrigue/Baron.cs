using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Baron : IntrigueCardModel
	{
		public Baron()
		{
			this.Type = CardType.Action;
			this.Name = "Baron";
			this.Cost = 4;
			this.Buys = 1;
		}

		private static string[] choices = new string[] { "Discard", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Discard an Estate", "Do nothing" };
		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			IEnumerable<CardModel> estates = currentPlayer.Hand.Where(card => card is Estate);

			if (estates.Any())
			{
				int discard = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardForBaron, "You may discard an Estate", choices, choiceDescriptions);
				if (discard == 0)
				{
					currentPlayer.DiscardCard(estates.First());
					currentPlayer.AddActionCoin(4);
				}
				else
				{
					currentPlayer.GainCard(typeof(Estate));
				}
			}
			else
			{
				currentPlayer.GainCard(typeof(Estate));
			}
		}

		public override CardModel Clone()
		{
			return new Baron();
		}
	}
}
