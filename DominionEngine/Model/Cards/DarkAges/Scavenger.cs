using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Scavenger : DarkAgesCardModel
	{
		public Scavenger()
		{
			this.Name = "Scavenger";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Coins = 2;
		}

		private static string[] choices = new string[] { "Yes", "No" };
		public override void Play(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.PutDeckInDiscard, "You may put your deck into the discard", choices, choices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.PutDeckInDiscard();
			}
			CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.PutOnDeckFromDiscard, "Put a card from the discard on top of your deck", Chooser.ChoiceSource.None, gameModel.CurrentPlayer.Discard);
			if (card != null)
			{
				gameModel.CurrentPlayer.RemoveFromDiscard(card);
				gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
			}
		}

		public override CardModel Clone()
		{
			return new Scavenger();
		}
	}
}