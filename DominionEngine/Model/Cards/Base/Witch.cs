using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Witch : BaseCardModel
	{
		public Witch()
		{
			this.Name = "Witch";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
			this.Cards = 2;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				player.GainCard(typeof(Curse));
			}
		}

		public override CardModel Clone()
		{
			return new Witch();
		}
	}
}
