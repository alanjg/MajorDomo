using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Market : BaseCardModel
	{
		public Market()
		{
			this.Name = "Market";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 1;
			this.Cards = 1;
			this.Buys = 1;
			this.Coins = 1;
		}

		public override CardModel Clone()
		{
			return new Market();
		}
	}
}
