using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class PhilosophersStone : AlchemyCardModel
	{
		public PhilosophersStone()
		{
			this.Name = "Philosopher's Stone";
			this.Type = CardType.Treasure;
			this.Cost = 3;
			this.CostsPotion = true;
		}

		public override void Play(GameModel gameModel)
		{
			int cards = gameModel.CurrentPlayer.Deck.Count + gameModel.CurrentPlayer.Discard.Count;
			int coin = cards / 5;
			gameModel.CurrentPlayer.AddActionCoin(coin);
		}

		public override CardModel Clone()
		{
			return new PhilosophersStone();
		}
	}
}
