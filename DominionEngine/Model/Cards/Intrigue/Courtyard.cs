using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Courtyard : IntrigueCardModel
	{
		public Courtyard()
		{
			this.Type = CardType.Action;
			this.Name = "Courtyard";
			this.Cost = 2;
			this.Cards = 3;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.PutOnDeckFromHand, this, "Put a card on the top of your deck", Chooser.ChoiceSource.FromHand, currentPlayer.Hand);
			if (choice != null)
			{
				currentPlayer.RemoveFromHand(choice);
				currentPlayer.Deck.PlaceOnTop(choice);
			}			
		}

		public override CardModel Clone()
		{
			return new Courtyard();
		}
	}
}
