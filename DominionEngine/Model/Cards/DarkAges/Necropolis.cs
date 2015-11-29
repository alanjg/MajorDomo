using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Necropolis : DarkAgesCardModel
	{
		public Necropolis()
		{
			this.Name = "Necropolis";
			this.Type = CardType.Action | CardType.Shelter;
			this.Cost = 1;
			this.Actions = 2;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Necropolis();
		}
	}
}