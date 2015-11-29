using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Dungeon : AdventuresCardModel
	{
		public Dungeon()
		{
			this.Name = "Dungeon";
			this.Type = CardType.Action | CardType.Duration;
			this.Cost = 3;
			
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			this.Cards = 2;
			gameModel.CurrentPlayer.DiscardCards(2);
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.Draw(2);
			gameModel.CurrentPlayer.DiscardCards(2);
		}

		public override CardModel Clone()
		{
			return new Dungeon();
		}
	}
}
