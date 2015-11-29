using System;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class Peddler : ProsperityCardModel
	{
		public Peddler()
		{
			this.Type = CardType.Action;
			this.Name = "Peddler";
			this.Cost = 8;
			this.Actions = 1;
			this.Coins = 1;
			this.Cards = 1;
		}

		public override CardModel Clone()
		{
			return new Peddler();
		}
	}
}