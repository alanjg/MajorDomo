using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Plan : AdventuresCardModel
	{
		public Plan()
		{
			this.Name = "Plan";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.CostsPotion = true;
			this.Actions = 1;
			this.Cards = 2;
		}

		public override CardModel Clone()
		{
			return new Plan();
		}
	}
}
