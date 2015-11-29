using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Smithy : BaseCardModel
	{
		public Smithy()
		{
			this.Name = "Smithy";
			this.Type = CardType.Action;
			this.Cards = 3;
			this.Cost = 4;
		}

		public override CardModel Clone()
		{
			return new Smithy();
		}
	}
}
