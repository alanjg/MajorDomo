using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class NomadCamp : HinterlandsCardModel
	{
		public NomadCamp()
		{
			this.Name = "Nomad Camp";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Coins = 2;
			this.Buys = 1;
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			player.RemoveFromDiscard(this);
			player.Deck.PlaceOnTop(this);
		}

		public override CardModel Clone()
		{
			return new NomadCamp();
		}
	}
}
