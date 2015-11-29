using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Ironmonger : DarkAgesCardModel
	{
		public Ironmonger()
		{
			this.Name = "Ironmonger";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Cards = 1;
			this.Actions = 1;
		}

		private static string[] choices = new string[] { "Keep", "Discard" };
		public override void Play(GameModel gameModel)
		{
			CardModel card = gameModel.CurrentPlayer.DrawCard();
			if (card != null)
			{
				gameModel.TextLog.WriteLine(gameModel.CurrentPlayer.Name + " reveals " + card.Name);
				int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardOrPutOnDeck, card, "You may discard it", choices, choices);
				if (choice == 0)
				{
					gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
				}
				else
				{
					gameModel.CurrentPlayer.DiscardCard(card);
				}

				if (card.Is(CardType.Action))
				{
					gameModel.CurrentPlayer.GainActions(1);
				}
				if (card.Is(CardType.Treasure))
				{
					gameModel.CurrentPlayer.AddActionCoin(1);
				}
				if (card.Is(CardType.Victory))
				{
					gameModel.CurrentPlayer.Draw();
				}
			}
		}

		public override CardModel Clone()
		{
			return new Ironmonger();
		}
	}
}