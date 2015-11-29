using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Seaway : AdventuresCardModel
	{
		public Seaway()
		{
			this.Name = "Seaway";
			this.Type = CardType.Event;
			this.Cost = 5;			
		}

		public override void OnBuy(GameModel gameModel)
		{
			IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(pile => pile.Count > 0 && gameModel.GetCost(pile) <= 4 && !pile.CostsPotion && pile.TopCard.Is(CardType.Action));
			Pile chosenPile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain an Action card costing up to $4", piles);
			if (chosenPile != null)
			{
				gameModel.CurrentPlayer.GainCard(chosenPile);
				gameModel.CurrentPlayer.BuyPile = chosenPile;
			}
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}
		public override CardModel Clone()
		{
			return new Seaway();
		}
	}
}
