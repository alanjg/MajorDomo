using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Steward : IntrigueCardModel
	{
		public Steward()
		{
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Name = "Steward";
		}

		private static string[] choices = new string[] { "Cards", "Coins", "Trash" };
		private static string[] choiceDescriptions = new string[] { "+2 Cards", "+2 Coin", "Trash 2 Cards" };
		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			int choice = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Steward, "Choose one", choices, choiceDescriptions);
			switch (choice)
			{
				case 0:
					currentPlayer.Draw();
					currentPlayer.Draw();
					break;
				case 1:
					currentPlayer.AddActionCoin(2);
					break;
				case 2:
					IEnumerable<CardModel> trashes = currentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.TrashFromHand, "Trash 2 cards", Chooser.ChoiceSource.FromHand, 2, 2, currentPlayer.Hand);

					foreach (CardModel card in trashes.ToList())
					{
						currentPlayer.Trash(card);
					}
					break;
			}
		}

		public override CardModel Clone()
		{
			return new Steward();
		}
	}
}
