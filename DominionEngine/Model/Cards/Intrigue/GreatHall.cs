using System;

namespace Dominion.Model.Actions
{
	public class GreatHall : IntrigueCardModel
	{
		public GreatHall()
		{
			this.Type = CardType.Action | CardType.Victory;
			this.Name = "Great Hall";
			this.Cost = 3;
			this.Cards = 1;
			this.Actions = 1;
			this.Points = 1;
		}

		public override CardModel Clone()
		{
			return new GreatHall();
		}
	}
}
