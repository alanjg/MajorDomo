using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Margrave : HinterlandsCardModel
	{
		public Margrave()
		{
			this.Name = "Margrave";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
			this.Cards = 3;
			this.Buys = 1;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player attacked in attackedPlayers)
			{
				attacked.Draw();
				attacked.DiscardTo(3);
			}
		}

		public override CardModel Clone()
		{
			return new Margrave();
		}
	}
}
