using System;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class Treasury : SeasideCardModel
	{
		public Treasury()
		{
			this.Type = CardType.Action;
			this.Name = "Treasury";
			this.Cost = 5;
			this.Actions = 1;
			this.Cards = 1;
			this.Coins = 1;
		}

		public override void OnCleanup(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromCleanUp(this);
			gameModel.CurrentPlayer.Deck.PlaceOnTop(this);
		}

		public override bool HasCleanupEffect(GameModel gameModel)
		{
			return !gameModel.CurrentPlayer.Bought.Any(card => card.Is(CardType.Victory));
		}

		public override CardModel Clone()
		{
			return new Treasury();
		}
	}
}