using System;

namespace Dominion.Model.Actions
{
	public class Bazaar : SeasideCardModel
	{
		public Bazaar()
		{
			this.Type = CardType.Action;
			this.Name = "Bazaar";
			this.Cost = 5;
			this.Actions = 2;
			this.Cards = 1;
			this.Coins = 1;
		}

		public override CardModel Clone()
		{
			return new Bazaar();
		}
	}
}