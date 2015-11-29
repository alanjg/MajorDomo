using System;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class Tactician : SeasideCardModel
	{
		private bool discardedACard;
		public Tactician()
		{
			this.Type = CardType.Action | CardType.Duration;
			this.Name = "Tactician";
			this.Cost = 5;
		}

		public override CardModel Clone()
		{
			Tactician clone = (Tactician)base.Clone();
			clone.discardedACard = this.discardedACard;
			return clone;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> hand = new List<CardModel>(gameModel.CurrentPlayer.Hand);
			foreach (CardModel card in hand)
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}
			this.discardedACard |= hand.Count > 0;
		}

		public override void PlayDuration(GameModel gameModel)
		{
			if (this.discardedACard)
			{
				gameModel.CurrentPlayer.GainBuys(1);
				gameModel.CurrentPlayer.GainActions(1);
				gameModel.CurrentPlayer.Draw(5);
				this.discardedACard = false;
			}
		}
	}
}