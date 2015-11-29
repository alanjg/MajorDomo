using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Rabble : ProsperityCardModel
	{
		public Rabble()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Rabble";
			this.Cost = 5;
			this.Cards = 3;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				List<CardModel> drawn = new List<CardModel>();
				for (int i = 0; i < 3; i++)
				{
					CardModel card = player.DrawCard();
					if (card != null)
					{
						if (card.Is(CardType.Action) || card.Is(CardType.Treasure))
						{
							player.DiscardCard(card);
						}
						else
						{
							drawn.Add(card);
						}
					}
				}
				IEnumerable<CardModel> order = player.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put them on your deck in any order(first on top)", drawn);
				foreach (CardModel toPutOnDeck in order.Reverse())
				{
					player.Deck.PlaceOnTop(toPutOnDeck);
				}				
			}
		}

		public override CardModel Clone()
		{
			return new Rabble();
		}
	}
}