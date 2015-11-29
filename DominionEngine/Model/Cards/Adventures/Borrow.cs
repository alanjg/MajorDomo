using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Borrow : AdventuresCardModel
	{
		public Borrow()
		{
			this.Name = "Borrow";
			this.Type = CardType.Event;
			this.Cost = 0;
		}

		public override void OnBuy(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainBuys(1);
			if (!gameModel.CurrentPlayer.HasUsedBorrow)
			{
				if (!gameModel.CurrentPlayer.HasMinusOneCardToken)
				{
					gameModel.CurrentPlayer.HasMinusOneCardToken = true;
					gameModel.CurrentPlayer.AddActionCoin(1);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Borrow();
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}
	}
}
