using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Possession : AlchemyCardModel
	{
		public Possession()
		{
			this.Name = "Possession";
			this.Type = CardType.Action;
			this.Cost = 6;
			this.CostsPotion = true;
		}

		public override void Play(GameModel gameModel)
		{
            gameModel.LeftOfCurrentPlayer.GainPossessionTurn();
		}

		public override CardModel Clone()
		{
			return new Possession();
		}
	}
}
