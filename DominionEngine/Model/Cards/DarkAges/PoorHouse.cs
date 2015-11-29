using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class PoorHouse : DarkAgesCardModel
	{
		public PoorHouse()
		{
			this.Name = "Poor House";
			this.Type = CardType.Action;
			this.Cost = 1;
			this.Coins = 4;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RevealHand();
			gameModel.CurrentPlayer.AddActionCoin(-Math.Min(gameModel.CurrentPlayer.Hand.Count(c => c.Is(CardType.Treasure)), gameModel.CurrentPlayer.Coin));
		}

		public override CardModel Clone()
		{
			return new PoorHouse();
		}
	}
}