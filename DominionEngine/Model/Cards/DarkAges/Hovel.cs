using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Hovel : DarkAgesCardModel
	{
		public Hovel()
		{
			this.Name = "Hovel";
			this.Type = CardType.Shelter | CardType.Reaction;
			this.Cost = 1;
			this.ReactionTrigger = Dominion.ReactionTrigger.VictoryCardBought;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Hovel();
		}
	}
}