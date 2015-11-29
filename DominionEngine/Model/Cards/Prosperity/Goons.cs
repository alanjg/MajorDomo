using System;
using System.Linq;
using Dominion.Model.Chooser;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class Goons : ProsperityCardModel
	{
		public Goons()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Goons";
			this.Cost = 6;
			this.Coins = 2;
			this.Buys = 1;
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
			return new Goons();
		}
	}
}