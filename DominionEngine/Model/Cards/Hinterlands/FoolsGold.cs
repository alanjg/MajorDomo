using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class FoolsGold : HinterlandsCardModel
	{
		public FoolsGold()
		{
			this.Name = "Fool's Gold";
			this.Type = CardType.Treasure | CardType.Reaction;
			this.Cost = 2;
			this.ReactionTrigger = ReactionTrigger.OpponentGainedProvince;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			if (currentPlayer.Played.Count(card => card is FoolsGold) > 1)
			{
				currentPlayer.AddActionCoin(4);
			}
			else
			{
				currentPlayer.AddActionCoin(1);
			}
		}

		public override CardModel Clone()
		{
			return new FoolsGold();
		}
	}
}
