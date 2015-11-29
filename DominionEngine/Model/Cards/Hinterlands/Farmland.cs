using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Farmland : HinterlandsCardModel
	{
		public Farmland()
		{
			this.Name = "Farmland";
			this.Type = CardType.Victory;
			this.Cost = 6;
			this.Points = 2;
		}

		public override void OnBuy(GameModel gameModel)
		{
			if (gameModel.CurrentPlayer.Hand.Count > 0)
			{
				CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForFarmland, "Trash a card", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				gameModel.CurrentPlayer.Trash(choice);
				IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(pile => gameModel.GetCost(pile) == gameModel.GetCost(choice) + 2 && choice.CostsPotion == pile.CostsPotion && pile.Count > 0);
				if (piles.Any())
				{
					Pile chosenPile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing $" + (gameModel.GetCost(choice) + 2), piles);
					gameModel.CurrentPlayer.GainCard(chosenPile);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Farmland();
		}
	}
}
