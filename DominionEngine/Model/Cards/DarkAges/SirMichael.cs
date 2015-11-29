using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class SirMichael : Knights
	{
		public SirMichael()
		{
			this.Name = "Sir Michael";
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			base.PlayAttack(gameModel, attackedPlayers);
			foreach (Player player in attackedPlayers)
			{
				player.DiscardTo(3);
			}
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new SirMichael();
		}
	}
}