using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class SilkRoad : HinterlandsCardModel
	{
		public SilkRoad()
		{
			this.Name = "Silk Road";
			this.Type = CardType.Victory;
			this.Cost = 4;
		}

		public override int GetVictoryPoints(Player player)
		{
			return player.AllCardsInDeck.Count(c => c.Is(CardType.Victory)) / 4;	
		}

		public override CardModel Clone()
		{
			return new SilkRoad();
		}
	}
}
