using System;

namespace Dominion.Model.Actions
{
	public class WorkersVillage : ProsperityCardModel
	{
		public WorkersVillage()
		{
			this.Type = CardType.Action;
			this.Name = "Worker's Village";
			this.Actions = 2;
			this.Cards = 1;
			this.Buys = 1;
			this.Cost = 4;
		}

		public override CardModel Clone()
		{
			return new WorkersVillage();
		}
	}
}