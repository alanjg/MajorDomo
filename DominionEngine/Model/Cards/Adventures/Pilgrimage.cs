using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Pilgrimage : AdventuresCardModel
	{
		public Pilgrimage()
		{
			this.Name = "Pilgrimage";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.CostsPotion = true;
			this.Actions = 1;
			this.Cards = 2;
		}

		public override CardModel Clone()
		{
			return new Pilgrimage();
		}
	}
}
