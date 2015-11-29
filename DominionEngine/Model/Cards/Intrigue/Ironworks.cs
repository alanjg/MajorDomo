using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Ironworks : IntrigueCardModel
	{
		public Ironworks()
		{
			this.Name = "Ironworks";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<Pile> piles = (from pile in gameModel.SupplyPiles where pile.Count > 0 && gameModel.GetCost(pile) <= 4 && !pile.CostsPotion select pile);
			Pile chosenPile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.GainForIronworks, "Gain a card costing up to $4", piles);
			if (chosenPile != null)
			{
				CardModel card = gameModel.CurrentPlayer.GainCard(chosenPile);
				if (card != null)
				{
					if (card.Is(CardType.Action))
					{
						gameModel.CurrentPlayer.GainActions(1);
					}
					if (card.Is(CardType.Treasure))
					{
						gameModel.CurrentPlayer.AddActionCoin(1);
					}
					if (card.Is(CardType.Victory))
					{
						gameModel.CurrentPlayer.Draw();
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Ironworks();
		}
	}
}
