using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Feast : BaseCardModel
	{
		public Feast()
		{
			this.Name = "Feast";
			this.Cost = 4;
			this.Type = CardType.Action;
		}

		public override void Play(GameModel gameModel)
		{
			Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing $5 or less", gameModel.SupplyPiles.Where(p => gameModel.GetCost(p) <= 5 && !p.CostsPotion && p.Count > 0));
			gameModel.CurrentPlayer.Trash(this);
			if (pile != null)
			{
				gameModel.CurrentPlayer.GainCard(pile);
			}
		}

		public override CardModel Clone()
		{
			return new Feast();
		}
	}
}
