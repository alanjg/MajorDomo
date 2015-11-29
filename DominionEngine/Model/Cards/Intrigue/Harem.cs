using System;

namespace Dominion.Model.Actions
{
	public class Harem : IntrigueCardModel
	{
		public Harem()
		{
			this.Type = CardType.Treasure | CardType.Victory; 
			this.Name = "Harem";
			this.Cost = 6;
			this.Coins = 2;
			this.Points = 2;
		}

		public override CardModel Clone()
		{
			return new Harem();
		}
	}
}
