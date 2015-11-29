using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class RuinedLibrary : DarkAgesCardModel
	{
		public RuinedLibrary()
		{
			this.Name = "Ruined Library";
			this.Type = CardType.Action | CardType.Ruins;
			this.Cost = 0;
			this.Cards = 1;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new RuinedLibrary();
		}
	}
}