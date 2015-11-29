using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Storeroom : DarkAgesCardModel
	{
		public Storeroom()
		{
			this.Name = "Storeroom";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Buys = 1;
		}

		public override void Play(GameModel gameModel)
		{
			IList<CardModel> choices = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.DiscardForCards, "Discard any number of cards.  +1 Card per card discarded.", Chooser.ChoiceSource.FromHand, 0, gameModel.CurrentPlayer.Hand.Count, gameModel.CurrentPlayer.Hand).ToList();
			foreach (CardModel card in choices)
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}
			gameModel.CurrentPlayer.Draw(choices.Count);
			choices = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.DiscardForCoins, "Discard any number of cards.  +1 Coin per card discarded.", Chooser.ChoiceSource.FromHand, 0, gameModel.CurrentPlayer.Hand.Count, gameModel.CurrentPlayer.Hand).ToList();
			foreach (CardModel card in choices)
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}
			gameModel.CurrentPlayer.AddActionCoin(choices.Count);
		}

		public override CardModel Clone()
		{
			return new Storeroom();
		}
	}
}