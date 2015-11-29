using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Embassy : HinterlandsCardModel
	{
		public Embassy()
		{
			this.Name = "Embassy";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			currentPlayer.Draw(5);
			currentPlayer.DiscardCards(3);
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			foreach (Player otherPlayer in gameModel.Players)
			{
				if (player != otherPlayer)
				{
					otherPlayer.GainCard(typeof(Silver));
				}
			}
		}

		public override CardModel Clone()
		{
			return new Embassy();
		}
	}
}
