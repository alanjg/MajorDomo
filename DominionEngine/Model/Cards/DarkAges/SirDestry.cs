using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class SirDestry : Knights
	{
		public SirDestry()
		{
			this.Name = "Sir Destry";
			this.Cards = 2;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new SirDestry();
		}
	}
}