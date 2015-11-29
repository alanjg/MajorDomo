using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Mission : AdventuresCardModel
	{
		public Mission()
		{
			this.Name = "Mission";
			this.Type = CardType.Event;
			this.Cost = 4;
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
			return new Mission();
		}
	}
}
