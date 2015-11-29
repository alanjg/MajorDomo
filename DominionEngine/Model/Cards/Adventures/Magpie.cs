using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Magpie : AdventuresCardModel
	{
		public Magpie()
		{
			this.Name = "Magpie";
			this.Type = CardType.Action;
			this.Cost = 4;
			
			this.Actions = 1;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel card = gameModel.CurrentPlayer.DrawCard();
			if (card != null)
			{
				gameModel.CurrentPlayer.RevealCard(card);
				if (card.Is(CardType.Treasure))
				{
					gameModel.CurrentPlayer.PutInHand(card);
				}
				else 
				{
					if (card.Is(CardType.Action) || card.Is(CardType.Victory))
					{
						gameModel.CurrentPlayer.GainCard(typeof(Magpie));
					}
					gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Magpie();
		}
	}
}
