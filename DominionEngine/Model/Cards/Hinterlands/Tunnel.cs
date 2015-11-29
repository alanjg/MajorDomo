using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Tunnel : HinterlandsCardModel
	{
		public Tunnel()
		{
			this.Name = "Tunnel";
			this.Type = CardType.Victory | CardType.Reaction;
			this.Cost = 3;
			this.ReactionTrigger = ReactionTrigger.CardDiscardedOutsideCleanup;
			this.Points = 2;
		}

		public override CardModel Clone()
		{
			return new Tunnel();
		}
	}
}
