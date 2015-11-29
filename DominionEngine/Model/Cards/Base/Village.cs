using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Village : BaseCardModel
	{
		public Village()
		{
			this.Name = "Village";
			this.Type = CardType.Action;
			this.Cards = 1;
			this.Actions = 2;
			this.Cost = 3;
		}

		public override CardModel Clone()
		{
			return new Village();
		}
	}
}
