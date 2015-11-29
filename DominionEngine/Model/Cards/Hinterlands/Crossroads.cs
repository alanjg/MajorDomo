using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Crossroads : HinterlandsCardModel
	{
		public const string CrossroadsName = "Crossroads";
		public Crossroads()
		{
			this.Name = CrossroadsName;
			this.Type = CardType.Action;
			this.Cost = 2;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			currentPlayer.RevealHand();

			int victoryCount = currentPlayer.Hand.Count(card => card.Is(CardType.Victory));
			currentPlayer.Draw(victoryCount);

			if (currentPlayer.Played.Count(card => card.Name == this.Name) <= 1)
			{
				currentPlayer.GainActions(3);
			}
		}

		public override CardModel Clone()
		{
			return new Crossroads();
		}
	}
}
