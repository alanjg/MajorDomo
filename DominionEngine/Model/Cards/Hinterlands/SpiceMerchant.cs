using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class SpiceMerchant : HinterlandsCardModel
	{
		public SpiceMerchant()
		{
			this.Name = "Spice Merchant";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		private static string[] trash = new string[] { "Yes", "No" };
		private static string[] type = new string[] { "Laboratory", "Woodcutter" };
		private static string[] typeDescriptions = new string[] { "+2 cards, +1 action", "+2 coins, +1 buy" };

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> treasures = gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Treasure));
			if (treasures.Any())
			{
				int trashATreasure = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.TrashForSpiceMerchant, "Trash a treasure?", trash, trash);
				if (trashATreasure == 0)
				{
					CardModel chosenTreasure = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashFromHand, "Trash a treasure", ChoiceSource.FromHand, treasures);
					gameModel.CurrentPlayer.Trash(chosenTreasure);
					int chosenType = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.SpiceMerchantEffect, "Choose an effect", type, typeDescriptions);
					if (chosenType == 0)
					{
						gameModel.CurrentPlayer.GainActions(1);
						gameModel.CurrentPlayer.Draw(2);
					}
					else
					{
						gameModel.CurrentPlayer.GainBuys(1);
						gameModel.CurrentPlayer.AddActionCoin(2);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new SpiceMerchant();
		}
	}
}
