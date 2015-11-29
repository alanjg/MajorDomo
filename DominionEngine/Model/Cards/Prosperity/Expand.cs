using System;
using System.Linq;
using System.Net;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Expand : ProsperityCardModel
	{
		public Expand()
		{
			this.Type = CardType.Action;
			this.Cost = 7;
			this.Name = "Expand";
		}
		
		public override void Play(GameModel gameModel)
		{
			CardModel trashedCard = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForExpand, "Choose a card to trash with Expand", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (trashedCard != null)
			{
				gameModel.CurrentPlayer.Trash(trashedCard);
				Pile newChoice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to $" + (gameModel.GetCost(trashedCard) + 3).ToString(),
					gameModel.SupplyPiles.Where(pile => gameModel.GetCost(pile) <= gameModel.GetCost(trashedCard) + 3 && pile.Count > 0 && (trashedCard.CostsPotion || !pile.CostsPotion)));

				if (newChoice != null)
				{
					gameModel.CurrentPlayer.GainCard(newChoice);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Expand();
		}
	}
}