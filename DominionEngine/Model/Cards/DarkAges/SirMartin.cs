using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class SirMartin : Knights
	{
		public SirMartin()
		{
			this.Name = "Sir Martin";
			this.Cost = 4;
			this.Buys = 2;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new SirMartin();
		}
	}
}