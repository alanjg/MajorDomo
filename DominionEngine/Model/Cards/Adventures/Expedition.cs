using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Expedition : AdventuresCardModel
	{
		public Expedition()
		{
			this.Name = "Amulet";
			this.Type = CardType.Event;
			this.Cost = 3;
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}

		public override void OnBuy(GameModel gameModel)
		{
			gameModel.CurrentPlayer.ExpeditionCount++;
		}

		public override CardModel Clone()
		{
			return new Expedition();
		}
	}
}
