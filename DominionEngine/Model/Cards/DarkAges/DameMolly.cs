using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class DameMolly : Knights
	{
		public DameMolly()
		{
			this.Name = "Dame Molly";
			this.Actions = 2;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new DameMolly();
		}
	}
}