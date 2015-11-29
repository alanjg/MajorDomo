using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Remake : CornucopiaCardModel
	{
		public Remake()
		{
			this.Name = "Remake";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			DoRemake(gameModel);
			DoRemake(gameModel);
		}

		private void DoRemake(GameModel gameModel)
		{
			CardModel trashedCard = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForRemake, "Trash a card", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (trashedCard != null)
			{
				gameModel.CurrentPlayer.Trash(trashedCard);
				Pile newChoice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing $" + (gameModel.GetCost(trashedCard) + 1).ToString(), gameModel.SupplyPiles.Where(pile =>
																gameModel.GetCost(pile) == gameModel.GetCost(trashedCard) + 1 && pile.Count > 0 &&
																(trashedCard.CostsPotion == pile.CostsPotion)));
				if (newChoice != null)
				{
					gameModel.CurrentPlayer.GainCard(newChoice);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Remake();
		}
	}
}
