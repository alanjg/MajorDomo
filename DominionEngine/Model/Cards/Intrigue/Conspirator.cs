using System;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class Conspirator : IntrigueCardModel
	{
		public Conspirator()
		{
			this.Type = CardType.Action;
			this.Name = "Conspirator";
			this.Cost = 4;
			this.Coins = 2;
		}		

		public override void Play(GameModel gameModel)
		{
			if (gameModel.CurrentPlayer.PlayedActions >= 3)
			{
				gameModel.CurrentPlayer.Draw();
				gameModel.CurrentPlayer.GainActions(1);
			}
		}

		public override CardModel Clone()
		{
			return new Conspirator();
		}
	}
}
