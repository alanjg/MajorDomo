using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Advisor : GuildsCardModel
	{
		public Advisor()
		{
			this.Name = "Advisor";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> top = gameModel.CurrentPlayer.DrawCards(3);
			gameModel.CurrentPlayer.RevealCards(top);
			CardModel chosen = gameModel.LeftOfCurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.ForceDiscard, "Choose one to discard", ChoiceSource.None, top);
			if (chosen != null)
			{
				gameModel.CurrentPlayer.DiscardCard(chosen);
			}
			foreach (CardModel card in top)
			{
				if (card != chosen)
				{
					gameModel.CurrentPlayer.PutInHand(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Advisor();
		}
	}
}
