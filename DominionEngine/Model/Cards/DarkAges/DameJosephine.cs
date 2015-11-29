using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class DameJosephine : Knights
	{
		public DameJosephine()
		{
			this.Name = "Dame Josephine";
			this.Type = CardType.Action | CardType.Attack | CardType.Knight | CardType.Victory;
			this.Points = 2;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new DameJosephine();
		}
	}
}