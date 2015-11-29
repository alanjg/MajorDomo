using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class DameSylvia : Knights
	{
		public DameSylvia()
		{
			this.Name = "Dame Sylvia";
			this.Coins = 2;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new DameSylvia();
		}
	}
}