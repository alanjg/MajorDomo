using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Haven : SeasideCardModel
	{
		public Haven()
		{
			this.Type = CardType.Action | CardType.Duration;
			this.Name = "Haven";
			this.Cost = 2;
			this.Actions = 1;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.Haven, "Choose a card to set aside", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (choice != null)
			{
				gameModel.CurrentPlayer.RemoveFromHand(choice);
				gameModel.CurrentPlayer.SetAsideForHaven(choice);
			}
		}

		public override CardModel Clone()
		{
			return new Haven();
		}
	}
}