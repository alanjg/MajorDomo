using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Urchin : DarkAgesCardModel
	{
		public new const string Name = "Urchin";
		public Urchin()
		{
			base.Name = "Urchin";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 3;
			this.Cards = 1;
			this.Actions = 1;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				player.DiscardTo(4);
			}
		}

		public override CardModel Clone()
		{
			return new Urchin();
		}
	}
}