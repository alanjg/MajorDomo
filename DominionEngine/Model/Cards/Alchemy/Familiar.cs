using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Familiar : AlchemyCardModel
	{
		public Familiar()
		{
			this.Name = "Familiar";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 3;
			this.CostsPotion = true;
			this.Actions = 1;
			this.Cards = 1;
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
			return new Familiar();
		}
	}
}
