using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Festival : BaseCardModel
	{
		public Festival()
		{
			this.Name = "Festival";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 2;
			this.Buys = 1;
			this.Coins = 2;
		}

		public override CardModel Clone()
		{
			return new Festival();
		}
	}
}
