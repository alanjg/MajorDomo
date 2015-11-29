using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class FarmingVillage : CornucopiaCardModel
	{
		public FarmingVillage()
		{
			this.Name = "Farming Village";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Actions = 2;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> discards = new List<CardModel>();
			CardModel drawn = gameModel.CurrentPlayer.DrawCard();
			while (drawn != null && !drawn.Is(CardType.Treasure) && !drawn.Is(CardType.Action))
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
			return new FarmingVillage();
		}
	}
}
