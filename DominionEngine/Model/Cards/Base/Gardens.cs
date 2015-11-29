using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Gardens : BaseCardModel
	{
		public Gardens()
		{
			this.Name = "Gardens";
			this.Type = CardType.Victory;
			this.Cost = 4;
		}

		public override int GetVictoryPoints(Player player)
		{
			return player.AllCardsInDeck.Count() / 10;
		}

		public override CardModel Clone()
		{
			return new Gardens();
		}
	}
}
