using System;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Loan : ProsperityCardModel
	{
		public Loan()
		{
			this.Type = CardType.Treasure;
			this.Name = "Loan";
			this.Cost = 3;
			this.Coins = 1;
		}

		public override bool AffectsTreasurePlayOrder
		{
			get
			{
				return true;
			}
		}
		private static string[] choices = new string[] { "Yes", "No" };
		public override void Play(GameModel gameModel)
		{
			List<CardModel> drawnCards = new List<CardModel>();
			CardModel drawnCard = gameModel.CurrentPlayer.DrawCard();
			while (drawnCard != null && !drawnCard.Is(CardType.Treasure))
			{
				drawnCards.Add(drawnCard);
				drawnCard = gameModel.CurrentPlayer.DrawCard();
			}
			if (drawnCard != null)
			{
				gameModel.TextLog.WriteLine(gameModel.CurrentPlayer.Name + " reveals " + drawnCard.Name);
				int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.TrashForLoan, drawnCard, "Trash it?", choices, choices);
				if (choice == 0)
				{
					gameModel.CurrentPlayer.Trash(drawnCard);
				}
				else
				{
					gameModel.CurrentPlayer.DiscardCard(drawnCard);
				}
			}
			foreach (CardModel card in drawnCards)
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}
		}

		public override CardModel Clone()
		{
			return new Loan();
		}
	}
}