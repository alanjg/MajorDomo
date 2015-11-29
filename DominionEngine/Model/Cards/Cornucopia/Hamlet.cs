using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Hamlet : CornucopiaCardModel
	{
		public Hamlet()
		{
			this.Name = "Hamlet";
			this.Type = CardType.Action;
			this.Cost = 2;
			this.Actions = 1;
			this.Cards = 1;
		}

		private static string[] actionChoices = new string[] { "Discard", "Nothing" };
		private static string[] actionChoiceDescriptions = new string[] { "Discard a card for +1 action", "Do nothing" };

		private static string[] buyChoices = new string[] { "Discard", "Nothing" };
		private static string[] buyChoiceDescriptions = new string[] { "Discard a card for +1 buy", "Do nothing" };
		
		public override void Play(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardForHamletAction, "You may discard a card for +1 action", actionChoices, actionChoiceDescriptions);
			if (choice == 0)
			{
				CardModel discardChoice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.Discard, "Discard a card", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				if (discardChoice != null)
				{
					gameModel.CurrentPlayer.DiscardCard(discardChoice);
					gameModel.CurrentPlayer.GainActions(1);
				}
			}

			choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardForHamletBuy, "You may discard a card for +1 buy", buyChoices, buyChoiceDescriptions);
			if (choice == 0)
			{
				CardModel discardChoice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.Discard, "Discard a card", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				if (discardChoice != null)
				{
					gameModel.CurrentPlayer.DiscardCard(discardChoice);
					gameModel.CurrentPlayer.GainBuys(1);
				}			
			}
		}

		public override CardModel Clone()
		{
			return new Hamlet();
		}
	}
}
