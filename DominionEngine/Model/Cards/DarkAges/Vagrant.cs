using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Dominion.Model.Actions
{
	public class Vagrant : DarkAgesCardModel
	{
		public Vagrant()
		{
			this.Name = "Vagrant";
			this.Type = CardType.Action;
			this.Cost = 2;
			this.Cards = 1;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel top = gameModel.CurrentPlayer.DrawCard();
			if (top != null)
			{
				string log = gameModel.CurrentPlayer.Name + " reveals " + top.Name;
				if (top.Is(CardType.Curse) || top.Is(CardType.Ruins) || top.Is(CardType.Shelter) || top.Is(CardType.Victory))
				{
					gameModel.CurrentPlayer.PutInHand(top);
					log += ", and puts it in hand.";
				}
				else
				{
					gameModel.CurrentPlayer.Deck.PlaceOnTop(top);
					log += ", and puts it back on the deck.";
				}
				gameModel.TextLog.WriteLine(log);
			}
		}

		public override CardModel Clone()
		{
			return new Vagrant();
		}
	}
}