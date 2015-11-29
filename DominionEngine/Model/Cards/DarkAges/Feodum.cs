using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Feodum : DarkAgesCardModel
	{
		public Feodum()
		{
			this.Name = "Feodum";
			this.Type = CardType.Victory;
			this.Cost = 4;
		}

		public override int GetVictoryPoints(Player player)
		{
			return player.AllCardsInDeck.Count(c => c is Silver) / 3;
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			for (int i = 0; i < 3; i++)
			{
				owner.GainCard(typeof(Silver));
			}
		}

		public override CardModel Clone()
		{
			return new Feodum();
		}
	}
}