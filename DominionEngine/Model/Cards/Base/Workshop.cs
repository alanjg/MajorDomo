using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Workshop : BaseCardModel
	{
		public Workshop()
		{
			this.Name = "Workshop";
			this.Type = CardType.Action;
			this.Cost = 3;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(pile => pile.Count > 0 && gameModel.GetCost(pile) <= 4 && !pile.CostsPotion);
			Pile chosenPile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to $4", piles);
			if (chosenPile != null)
			{
				gameModel.CurrentPlayer.GainCard(chosenPile);
			}
		}

		public override CardModel Clone()
		{
			return new Workshop();
		}
	}
}
