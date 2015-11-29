using System;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Envoy : PromoCardModel
	{
		public Envoy()
		{
			this.Type = CardType.Action;
			this.Name = "Envoy";
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> cards = gameModel.CurrentPlayer.DrawCards(5);
			if (cards.Count > 0)
			{
				gameModel.TextLog.Write(gameModel.CurrentPlayer.Name + " reveals ");
				gameModel.TextLog.WriteSortedCards(cards);
				gameModel.TextLog.WriteLine();

				CardModel choice = gameModel.LeftOfCurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.ForceDiscard, "Choose one card to discard", Chooser.ChoiceSource.None, cards);
				cards.Remove(choice);
				gameModel.CurrentPlayer.DiscardCard(choice);

				foreach (CardModel card in cards)
				{
					gameModel.CurrentPlayer.PutInHand(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Envoy();
		}
	}
}