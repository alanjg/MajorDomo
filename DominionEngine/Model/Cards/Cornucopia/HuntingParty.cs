using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class HuntingParty : CornucopiaCardModel
	{
		public HuntingParty()
		{
			this.Name = "Hunting Party";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 1;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> discards = new List<CardModel>();
			CardModel drawn = gameModel.CurrentPlayer.DrawCard();
			while (drawn != null && gameModel.CurrentPlayer.Hand.Any(card => card.GetType() == drawn.GetType()))
			{
				discards.Add(drawn);
				drawn = gameModel.CurrentPlayer.DrawCard();
			}
			if (drawn != null)
			{
				gameModel.CurrentPlayer.PutInHand(drawn);
			}
			foreach (CardModel discard in discards)
			{
				gameModel.CurrentPlayer.DiscardCard(discard);
			}
		}

		public override CardModel Clone()
		{
			return new HuntingParty();
		}
	}
}
