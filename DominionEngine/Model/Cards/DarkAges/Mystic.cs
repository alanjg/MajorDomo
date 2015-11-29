using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Mystic : DarkAgesCardModel
	{
		public Mystic()
		{
			this.Name = "Mystic";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 1;
			this.Coins = 2;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.NameACardToDraw, "Name a card", Chooser.ChoiceSource.None, gameModel.AllCardsInGame);
			CardModel topCard = gameModel.CurrentPlayer.DrawCard();
			if (topCard != null)
			{
				string log = gameModel.CurrentPlayer.Name + " reveals " + topCard.Name;
				if (topCard.Name == choice.Name)
				{
					log += ", and puts it in hand.";
					gameModel.CurrentPlayer.PutInHand(topCard);
				}
				else
				{
					gameModel.CurrentPlayer.Deck.PlaceOnTop(topCard);
				}
				gameModel.TextLog.WriteLine(log);
			}
			else
			{
				gameModel.TextLog.WriteLine(gameModel.CurrentPlayer.Name + " has no cards to reveal.");
			}
		}

		public override CardModel Clone()
		{
			return new Mystic();
		}
	}
}