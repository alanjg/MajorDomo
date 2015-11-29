using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class TravellingFair : AdventuresCardModel
	{
		public TravellingFair()
		{
			this.Name = "Travelling Fair";
			this.Type = CardType.Event;
			this.Cost = 2;
			
			this.Buys = 2;
		}

		public override void OnBuy(GameModel gameModel)
		{
			gameModel.CurrentPlayer.HasTravellingFairEffect = true;
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
			return new TravellingFair();
		}
	}
}
