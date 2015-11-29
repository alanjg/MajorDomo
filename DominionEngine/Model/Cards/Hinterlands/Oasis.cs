using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Oasis : HinterlandsCardModel
	{
		public Oasis()
		{
			this.Name = "Oasis";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Coins = 1;
			this.Cards = 1;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.DiscardCards(1);
		}

		public override CardModel Clone()
		{
			return new Oasis();
		}
	}
}
