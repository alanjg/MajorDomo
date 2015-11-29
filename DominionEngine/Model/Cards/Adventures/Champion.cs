using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Champion : AdventuresCardModel
	{
		public new const string Name = "Champion";
		public Champion()
		{
			base.Name = Champion.Name;
			this.Type = CardType.Action | CardType.Duration;
			this.Cost = 6;
			this.Actions = 1;
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}

		public override CardModel Clone()
		{
			return new Champion();
		}
	}
}
