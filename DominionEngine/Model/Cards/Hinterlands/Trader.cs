using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Trader : HinterlandsCardModel
	{
		public Trader()
		{
			this.Name = "Trader";
			this.Type = CardType.Action | CardType.Reaction;
			this.Cost = 4;
			this.ReactionTrigger = ReactionTrigger.CardGained;
		}

		public override void Play(GameModel gameModel)
		{
			if (gameModel.CurrentPlayer.Hand.Count > 0)
			{
				CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForTrader, "Trash a card", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				int cost = gameModel.GetCost(choice);
				gameModel.CurrentPlayer.Trash(choice);
				for (int i = 0; i < cost; i++)
				{
					gameModel.CurrentPlayer.GainCard(typeof(Silver));
				}
			}
		}

		public override CardModel Clone()
		{
			return new Trader();
		}
	}
}
