using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Vineyard : AlchemyCardModel
	{
		public Vineyard()
		{
			this.Name = "Vineyard";
			this.Type = CardType.Victory;
			this.Cost = 0;
			this.CostsPotion = true;
		}

		public override int GetVictoryPoints(Player player)
		{
			int actions = 0;
			foreach(CardModel card in player.AllCardsInDeck)
			{
				if (card.Is(CardType.Action))
				{
					actions++;
				}
			}
			return actions / 3;
		}

		public override CardModel Clone()
		{
			return new Vineyard();
		}
	}
}
