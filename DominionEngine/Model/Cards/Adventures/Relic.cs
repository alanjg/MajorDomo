using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Relic : AdventuresCardModel
	{
		public Relic()
		{
			this.Name = "Relic";
			this.Type = CardType.Treasure | CardType.Attack;
			this.Cost = 5;
			this.Coins = 2;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach(Player player in attackedPlayers)
			{
				player.HasMinusOneCoinToken = true;
			}
		}

		public override CardModel Clone()
		{
			return new Relic();
		}
	}
}
