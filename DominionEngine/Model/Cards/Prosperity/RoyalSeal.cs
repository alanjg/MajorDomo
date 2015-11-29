using System;

namespace Dominion.Model.Actions
{
	public class RoyalSeal : ProsperityCardModel
	{
		public RoyalSeal()
		{
			this.Type = CardType.Treasure;
			this.Name = "Royal Seal";
			this.Cost = 5;
			this.Coins = 2;
		}

		public override CardModel Clone()
		{
			return new RoyalSeal();
		}
	}
}