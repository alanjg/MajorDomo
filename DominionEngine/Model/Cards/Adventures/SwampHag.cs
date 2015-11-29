using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class SwampHag : AdventuresCardModel
	{
		public SwampHag()
		{
			this.Name = "Swamp Hag";
			this.Type = CardType.Action | CardType.Attack | CardType.Duration;
			this.Cost = 5;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player otherPlayer in attackedPlayers)
			{
				int count = 0;
				otherPlayer.HasSwampHagEffect.TryGetValue(gameModel.CurrentPlayer, out count);
				count++;
				otherPlayer.HasSwampHagEffect[gameModel.CurrentPlayer] = count;
			}
		}
		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddActionCoin(3);
		}

		public override CardModel Clone()
		{
			return new SwampHag();
		}
	}
}
