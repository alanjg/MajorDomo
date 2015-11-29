using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class HauntedWoods : AdventuresCardModel
	{
		public HauntedWoods()
		{
			this.Name = "HauntedWoods";
			this.Type = CardType.Action | CardType.Duration | CardType.Attack;
			this.Cost = 5;
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.Draw(3);
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach(Player player in attackedPlayers)
			{
				player.HasHauntedWoodsEffect.Add(gameModel.CurrentPlayer);
			}
		}

		public override CardModel Clone()
		{
			return new HauntedWoods();
		}
	}
}
