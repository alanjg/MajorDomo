using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class OvergrownEstate : DarkAgesCardModel
	{
		public OvergrownEstate()
		{
			this.Name = "Overgrown Estate";
			this.Type = CardType.Victory | CardType.Shelter;
			this.Cost = 1;
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			owner.Draw();
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new OvergrownEstate();
		}
	}
}