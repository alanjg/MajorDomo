using System;
using System.Linq;
namespace Dominion.Model.Actions
{
	public class WalledVillage : PromoCardModel
	{
		public WalledVillage()
		{
			this.Type = CardType.Action;
			this.Name = "Walled Village";
			this.Cost = 4;
			this.Actions = 2;
			this.Cards = 1;
		}

		public override bool HasCleanupEffect(GameModel gameModel)
		{
			return gameModel.CurrentPlayer.Played.Where(c => c.Is(CardType.Action)).Count() <= 2;
		}

		public override void OnCleanup(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromCleanUp(this.ThisAsTrashTarget);
			this.ThisAsTrashTarget.OnRemovedFromPlay(gameModel);
			gameModel.CurrentPlayer.Deck.PlaceOnTop(this.ThisAsTrashTarget);
		}

		public override CardModel Clone()
		{
			return new WalledVillage();
		}
	}
}