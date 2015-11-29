using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Count : DarkAgesCardModel
	{
		public Count()
		{
			this.Name = "Count";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		private static string[] choice1 = new string[] { "Discard", "PutBack", "GainCopper" };
		private static string[] choiceText1 = new string[] { "Discard 2 cards", "Put a card from hand on top of your deck", "Gain a Copper" };

		private static string[] choice2 = new string[] { "Coin", "Trash", "Duchy" };
		private static string[] choiceText2 = new string[] { "+3 Coin", "Trash your hand", "Gain a Duchy" };

		public override void Play(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.CountEffect1, "Choose one", choice1, choiceText1);
			switch (choice)
			{
				case 0:
					IEnumerable<CardModel> choices = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.Discard, choiceText1[0], Chooser.ChoiceSource.FromHand, 2, 2, gameModel.CurrentPlayer.Hand);
					foreach (CardModel card in choices.ToArray())
					{
						gameModel.CurrentPlayer.DiscardCard(card);
					}
					break;
				case 1:
					CardModel c = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.PutOnDeckFromHand, this, choiceText1[1], Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
					if (c != null)
					{
						gameModel.CurrentPlayer.RemoveFromHand(c);
						gameModel.CurrentPlayer.Deck.PlaceOnTop(c);
					}
					break;
				case 2:
					gameModel.CurrentPlayer.GainCard(typeof(Copper));
					break;
			}
			choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.CountEffect2, "Choose one", choice2, choiceText2);
		
			switch (choice)
			{
				case 0:
					gameModel.CurrentPlayer.AddActionCoin(3);
					break;
				case 1:
					foreach (CardModel card in gameModel.CurrentPlayer.Hand.ToArray())
					{
						gameModel.CurrentPlayer.Trash(card);
					}
					break;
				case 2:
					gameModel.CurrentPlayer.GainCard(typeof(Duchy));
					break;
			}
		}

		public override CardModel Clone()
		{
			return new Count();
		}
	}
}