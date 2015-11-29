using System;

namespace Dominion.Model.Actions
{
	public class Hoard : ProsperityCardModel
	{
		public Hoard()
		{
			this.Type = CardType.Treasure;
			this.Name = "Hoard";
			this.Cost = 6;
			this.Coins = 2;
		}

		public override CardModel Clone()
		{
			return new Hoard();
		}
	}
}