using System;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class ShantyTown : IntrigueCardModel
	{
		public ShantyTown()
		{
			this.Type = CardType.Action;
			this.Name = "Shanty Town";
			this.Cost = 3;
			this.Actions = 2;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			currentPlayer.RevealHand();
			if (!currentPlayer.Hand.Any(card => card.Is(CardType.Action)))
			{
				currentPlayer.Draw(2);
			}
		}

		public override CardModel Clone()
		{
			return new ShantyTown();
		}
	}
}
