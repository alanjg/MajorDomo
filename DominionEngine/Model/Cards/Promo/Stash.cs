using System;

namespace Dominion.Model.Actions
{
	public class Stash : PromoCardModel
	{
		public Stash()
		{
			this.Type = CardType.Treasure;
			this.Name = "Stash";
			this.Cost = 5;
			this.Coins = 2;
		}

		public override CardModel Clone()
		{
			return new Stash();
		}
	}
}