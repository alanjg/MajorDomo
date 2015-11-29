using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Develop : HinterlandsCardModel
	{
		public Develop()
		{
			this.Name = "Develop";
			this.Type = CardType.Action;
			this.Cost = 3;
		}

		public override void Play(GameModel gameModel)
		{
			if (gameModel.CurrentPlayer.Hand.Any())
			{
				CardModel trashedCard = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForDevelop, "Trash a card", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				int cost = 0;
				gameModel.CurrentPlayer.Trash(trashedCard);
				IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(pile => Math.Abs(gameModel.GetCost(pile) - gameModel.GetCost(trashedCard)) == 1 && pile.Count > 0 &&
																				(trashedCard.CostsPotion == pile.CostsPotion));
				if (piles.Any())
				{
					string costString = (gameModel.GetCost(trashedCard) + 1).ToString();
					if (gameModel.GetCost(trashedCard) - 1 >= 0)
					{
						costString += " or $" + (gameModel.GetCost(trashedCard) - 1).ToString();
					}
					Pile newChoice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.GainOnTopOfDeck, "Gain a card costing $" + costString, piles);
					if (newChoice != null)
					{
						gameModel.CurrentPlayer.GainCard(newChoice, GainLocation.TopOfDeck);
						cost = newChoice.GetCost();
					}

					int otherCost = cost == gameModel.GetCost(trashedCard) + 1 ? cost - 2 : cost + 2;
					IEnumerable<Pile> otherPiles = gameModel.SupplyPiles.Where(pile => gameModel.GetCost(pile) == otherCost && pile.Count > 0 &&
																					(trashedCard.CostsPotion == pile.CostsPotion));
					if (otherPiles.Any())
					{
						newChoice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.GainOnTopOfDeck, "Gain a card costing $" + otherCost, otherPiles);
						if (newChoice != null)
						{
							gameModel.CurrentPlayer.GainCard(newChoice, GainLocation.TopOfDeck);
						}
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Develop();
		}
	}
}
