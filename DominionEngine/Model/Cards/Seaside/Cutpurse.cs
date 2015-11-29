using System;
using System.Linq;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class Cutpurse : SeasideCardModel
	{
		public Cutpurse()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Cutpurse";
			this.Cost = 4;
			this.Coins = 2;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				CardModel copper = player.Hand.FirstOrDefault(card => card is Copper);
				if (copper != null)
				{
					player.DiscardCard(copper);
				}
				else
				{
					player.RevealHand();
				}
			}
		}

		public override CardModel Clone()
		{
			return new Cutpurse();
		}
	}
}