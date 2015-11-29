using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Ferry : AdventuresCardModel
	{
		public Ferry()
		{
			this.Name = "Ferry";
			this.Type = CardType.Event;
			this.Cost = 3;
		}

		public override void OnBuy(GameModel gameModel)
		{
			Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(Chooser.CardChoiceType.Ferry, "Move your -$2 token to an Action Supply pile.", gameModel.SupplyPiles.Where(p => p.Card.Is(CardType.Action)));
			if (pile != null)
			{
				gameModel.CurrentPlayer.FerryPile = pile;
				gameModel.UpdateSupplyPileCosts();
			}
		}

		public override CardModel Clone()
		{
			return new Ferry();
		}
	}
}
