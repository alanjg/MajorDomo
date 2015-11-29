using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Gear : AdventuresCardModel
	{
		public Gear()
		{
			this.Name = "Gear";
			this.Type = CardType.Action | CardType.Duration;
			this.Cost = 3;

			this.Cards = 2;
		}

		public override void Play(GameModel gameModel)
		{
			foreach(CardModel choice in gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.Haven, "Choose up to two cards to set aside", Chooser.ChoiceSource.FromHand, 0, 2, gameModel.CurrentPlayer.Hand))
			{ 
				gameModel.CurrentPlayer.RemoveFromHand(choice);
				gameModel.CurrentPlayer.SetAsideForHaven(choice);
			}
		}

		public override CardModel Clone()
		{
			return new Gear();
		}
	}
}
