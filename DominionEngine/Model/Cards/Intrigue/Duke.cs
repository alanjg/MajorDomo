using System;
using System.Linq;
using System.Net;

namespace Dominion.Model.Actions
{
	public class Duke : IntrigueCardModel
	{
		public Duke()
		{
			this.Type = CardType.Victory;
			this.Name = "Duke";
			this.Cost = 5;
		}

		public override int GetVictoryPoints(Player player)
		{
			return player.AllCardsInDeck.Count(card => card is Duchy);
		}

		public override CardModel Clone()
		{
			return new Duke();
		}
	}
}
