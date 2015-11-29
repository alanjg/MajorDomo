using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Island : SeasideCardModel
	{
		public Island()
		{
			this.Type = CardType.Action | CardType.Victory;
			this.Name = "Island";
			this.Cost = 4;
			this.Points = 2;
		}

		public override void Play(GameModel gameModel)
		{
			Player player = gameModel.CurrentPlayer;
			CardModel choice = player.Chooser.ChooseOneCard(CardChoiceType.Island, "Choose a card to set aside with Island", Chooser.ChoiceSource.FromHand, player.Hand);
			if (choice != null)
			{
				player.RemoveFromHand(choice);
			}
			this.ThisAsTrashTarget.OnRemovedFromPlay(gameModel);
			player.RemoveFromPlayed(this.ThisAsTrashTarget);
			player.PutOnIsland(this.ThisAsTrashTarget, choice);
		}

		public override CardModel Clone()
		{
			return new Island();
		}
	}
}