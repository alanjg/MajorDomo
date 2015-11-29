using System;

namespace Dominion.Model.Actions
{
	public class Talisman : ProsperityCardModel
	{
		public Talisman()
		{
			this.Type = CardType.Treasure;
			this.Name = "Talisman";
			this.Cost = 4;
			this.Coins = 1;
		}

		public override CardModel Clone()
		{
			return new Talisman();
		}
	}
}