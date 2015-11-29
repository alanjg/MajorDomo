using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class DameNatalie : Knights
	{
		public DameNatalie()
		{
			this.Name = "Dame Natalie";
		}

		public override void Play(GameModel gameModel)
		{
			Pile pile = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOnePile(CardChoiceType.Gain, "You may gain a card costing up to $3", gameModel.SupplyPiles.Where(p => p.Count > 0 && p.GetCost() <= 3 && !p.CostsPotion));
			if(pile != null)
			{
				gameModel.CurrentPlayer.GainCard(pile);
			}
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new DameNatalie();
		}
	}
}