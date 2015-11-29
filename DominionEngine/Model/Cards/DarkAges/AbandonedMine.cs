using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class AbandonedMine : DarkAgesCardModel
	{
		public AbandonedMine()
		{
			this.Name = "Abandoned Mine";
			this.Type = CardType.Action | CardType.Ruins;
			this.Cost = 0;
			this.Coins = 1;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new AbandonedMine();
		}
	}
}