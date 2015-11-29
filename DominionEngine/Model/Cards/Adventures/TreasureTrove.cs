using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class TreasureTrove : AdventuresCardModel
	{
		public TreasureTrove()
		{
			this.Name = "Treasure Trove";
			this.Type = CardType.Treasure;
			this.Cost = 5;
			
			this.Coins = 2;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Gold));
			gameModel.CurrentPlayer.GainCard(typeof(Copper));
		}

		public override CardModel Clone()
		{
			return new TreasureTrove();
		}
	}
}
