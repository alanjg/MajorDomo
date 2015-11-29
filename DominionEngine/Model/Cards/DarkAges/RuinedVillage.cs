using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class RuinedVillage : DarkAgesCardModel
	{
		public RuinedVillage()
		{
			this.Name = "Ruined Village";
			this.Type = CardType.Action | CardType.Ruins;
			this.Cost = 0;
			this.Actions = 1;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new RuinedVillage();
		}
	}
}