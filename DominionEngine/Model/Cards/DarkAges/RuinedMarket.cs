using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class RuinedMarket : DarkAgesCardModel
	{
		public RuinedMarket()
		{
			this.Name = "Ruined Market";
			this.Type = CardType.Action | CardType.Ruins;
			this.Cost = 0;
			this.Buys = 1;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new RuinedMarket();
		}
	}
}