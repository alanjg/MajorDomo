using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Harvest : CornucopiaCardModel
	{
		public Harvest()
		{
			this.Name = "Harvest";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> drawn = gameModel.CurrentPlayer.DrawCards(4);
			gameModel.CurrentPlayer.AddActionCoin(drawn.Select(card => card.Name).Distinct().Count());
			foreach (CardModel draw in drawn)
			{
				gameModel.CurrentPlayer.DiscardCard(draw);
			}
		}

		public override CardModel Clone()
		{
			return new Harvest();
		}
	}
}
