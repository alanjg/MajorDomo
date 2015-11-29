using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Laboratory : BaseCardModel
	{
		public Laboratory()
		{
			this.Name = "Laboratory";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 1;
			this.Cards = 2;
		}

		public override CardModel Clone()
		{
			return new Laboratory();
		}
	}
}
