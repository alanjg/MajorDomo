using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Ranger : AdventuresCardModel
	{
		public Ranger()
		{
			this.Name = "Ranger";
			this.Type = CardType.Action;
			this.Cost = 4;

			this.Buys = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.JourneyTokenIsFaceUp = !gameModel.CurrentPlayer.JourneyTokenIsFaceUp;
			if (gameModel.CurrentPlayer.JourneyTokenIsFaceUp)
			{
				gameModel.CurrentPlayer.Draw(5);
			}
		}

		public override CardModel Clone()
		{
			return new Ranger();
		}
	}
}
