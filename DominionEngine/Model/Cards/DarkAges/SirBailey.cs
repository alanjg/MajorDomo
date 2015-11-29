using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class SirBailey : Knights
	{
		public SirBailey()
		{
			this.Name = "Sir Bailey";
			this.Cards = 1;
			this.Actions = 1;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new SirBailey();
		}
	}
}