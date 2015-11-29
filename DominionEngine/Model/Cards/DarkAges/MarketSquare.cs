using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class MarketSquare : DarkAgesCardModel
	{
		public MarketSquare()
		{
			this.Name = "Market Square";
			this.Type = CardType.Action | CardType.Reaction;
			this.Cost = 3;
			this.Cards = 1;
			this.Actions = 1;
			this.Buys = 1;
			this.ReactionTrigger = ReactionTrigger.OwnerCardTrashed;
		}

		public override CardModel Clone()
		{
			return new MarketSquare();
		}
	}
}