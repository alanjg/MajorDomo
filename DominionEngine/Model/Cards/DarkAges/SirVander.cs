using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class SirVander : Knights
	{
		public SirVander()
		{
			this.Name = "Sir Vander";
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			owner.GainCard(typeof(Gold));
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new SirVander();
		}
	}
}