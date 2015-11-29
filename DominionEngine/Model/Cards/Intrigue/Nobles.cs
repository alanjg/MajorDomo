using System;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Nobles : IntrigueCardModel
	{
		public Nobles()
		{
			this.Name = "Nobles";
			this.Type = CardType.Action | CardType.Victory;
			this.Cost = 6;
			this.Points = 2;
		}

		private static string[] choices = new string[] { "Actions", "Cards" };
		private static string[] choiceDescriptions = new string[] { "+2 Actions", "+3 Cards" };

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			int choice = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Nobles, "Choose one:", choices, choiceDescriptions);
			if (choice == 0)
			{
				currentPlayer.GainActions(2);
			}
			else
			{
				currentPlayer.Draw(3);
			}
		}

		public override CardModel Clone()
		{
			return new Nobles();
		}
	}
}
