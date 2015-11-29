using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Haggler : HinterlandsCardModel
	{
		public Haggler()
		{
			this.Name = "Haggler";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Coins = 2;
		}

		public override CardModel Clone()
		{
			return new Haggler();
		}
	}
}
