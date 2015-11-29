using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Militia : BaseCardModel
	{
		public Militia()
		{
			this.Name = "Militia";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 4;
			this.Coins = 2;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				player.DiscardTo(3);
			}
		}

		public override CardModel Clone()
		{
			return new Militia();
		}
	}
}
