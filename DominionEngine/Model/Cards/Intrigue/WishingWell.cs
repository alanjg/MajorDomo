using System;
using System.Linq;
using Dominion.Model.Chooser;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class WishingWell : IntrigueCardModel
	{
		public WishingWell()
		{
			this.Type = CardType.Action;
			this.Name = "Wishing Well";
			this.Cost = 3;
			this.Actions = 1;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			
			CardModel guess = currentPlayer.Chooser.ChooseOneCard(CardChoiceType.NameACardToDraw, "Wish for a card", ChoiceSource.FromPile | ChoiceSource.None, gameModel.AllCardsInGame);
			gameModel.TextLog.WriteLine(currentPlayer.Name + " names " + guess.Name);
			CardModel card = currentPlayer.DrawCard();
			if (card != null)
			{
				gameModel.TextLog.WriteLine(currentPlayer.Name + " reveals " + card.Name);
				if (card.Name == guess.Name)
				{
					currentPlayer.PutInHand(card);
				}
				else
				{
					currentPlayer.Deck.PlaceOnTop(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new WishingWell();
		}
	}
}
