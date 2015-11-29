using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Woodcutter : BaseCardModel
	{
		public Woodcutter()
		{
			this.Name = "Woodcutter";
			this.Buys = 1;
			this.Cost = 3;
			this.Type = CardType.Action;
			this.Coins = 2;
		}

		public override CardModel Clone()
		{
			return new Woodcutter();
		}
	}
}
