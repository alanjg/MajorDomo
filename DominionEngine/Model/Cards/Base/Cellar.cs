using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Cellar : BaseCardModel
	{
		public Cellar()
		{
			this.Name = "Cellar";
			this.Type = CardType.Action;
			this.Actions = 1;
			this.Cost = 2;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> cards = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.DiscardForCellar, "Choose cards to discard", ChoiceSource.FromHand, 0, gameModel.CurrentPlayer.Hand.Count, gameModel.CurrentPlayer.Hand);
			int discarded = 0;

			foreach (CardModel card in cards.ToList())
			{
				gameModel.CurrentPlayer.DiscardCard(card);
				discarded++;
			}

			gameModel.CurrentPlayer.Draw(discarded);
		}

		public override CardModel Clone()
		{
			return new Cellar();
		}
	}
}
