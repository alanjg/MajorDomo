using System;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Pawn : IntrigueCardModel
	{
		public Pawn()
		{
			this.Name = "Pawn";
			this.Cost = 2;
			this.Type = CardType.Action;
		}

		private static string[] choices = new string[] { "Card", "Coin", "Action", "Buy" };
		private static string[] choiceDescriptions = new string[] { "+1 Card", "+1 Coin", "+1 Action", "+1 Buy" };
		
		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			IEnumerable<int> c = currentPlayer.Chooser.ChooseSeveralEffects(EffectChoiceType.Pawn, "Choose 2", 2, 2, choices, choiceDescriptions);
			foreach (int choice in c)
			{
				switch (choice)
				{
					case 0:
						currentPlayer.Draw();
						break;

					case 1:
						currentPlayer.AddActionCoin(1);
						break;

					case 2:
						currentPlayer.GainActions(1);
						break;

					case 3:
						currentPlayer.GainBuys(1);
						break;
				}
			}
		}

		public override CardModel Clone()
		{
			return new Pawn();
		}
	}
}
