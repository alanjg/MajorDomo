using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Masquerade : IntrigueCardModel
	{
		public Masquerade()
		{
			this.Name = "Masquerade";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Cards = 2;
		}

		private static string[] choices = new string[] { "Trash", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Trash a card", "Do nothing" };
		public override void Play(GameModel gameModel)
		{
			int currentPlayerIndex = gameModel.Players.IndexOf(gameModel.CurrentPlayer);
			CardModel[] passed = new CardModel[gameModel.Players.Count];
			int index = 0;
			foreach (Player player in gameModel.Players)
			{
				CardModel passedCard = player.Chooser.ChooseOneCard(CardChoiceType.Masquerade, "Pass a card", ChoiceSource.FromHand, player.Hand);
				player.RemoveFromHand(passedCard);
				passed[(index + 1) % gameModel.Players.Count] = passedCard;
				index++;
			}
			
			for (int i = 0; i < gameModel.Players.Count; i++)
			{
				if (passed[i] != null)
				{
					gameModel.Players[i].PutInHand(passed[i]);
				}
				gameModel.Players[i].OnPassedCard();
			}
			if (gameModel.CurrentPlayer.Hand.Count > 0)
			{
				CardModel card = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashFromHand, "You may trash a card from hand", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				if (card != null)
				{
					gameModel.CurrentPlayer.Trash(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Masquerade();
		}
	}
}
