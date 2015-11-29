using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Fairgrounds : CornucopiaCardModel
	{
		public Fairgrounds()
		{
			this.Name = "Fairgrounds";
			this.Type = CardType.Victory;
			this.Cost = 6;
		}

		public override int GetVictoryPoints(Player player)
		{
			return 2 * (player.AllCardsInDeck.Select(card => card.Name).Distinct().Count() / 5);
		}

		public override CardModel Clone()
		{
			return new Fairgrounds();
		}
	}
}
