using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class CountingHouse : ProsperityCardModel
	{
		public CountingHouse()
		{
			this.Type = CardType.Action;
			this.Name = "Counting House";
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> coppers = gameModel.CurrentPlayer.Discard.Where(card => card is Copper);
			IEnumerable<CardModel> choices = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.CountingHouse, "Choose coppers to place in hand", Chooser.ChoiceSource.None, 0, coppers.Count(), coppers);

			foreach (CardModel copper in choices.ToList())
			{
				gameModel.CurrentPlayer.RemoveFromDiscard(copper);
				gameModel.CurrentPlayer.PutInHand(copper);
			}
		}

		public override CardModel Clone()
		{
			return new CountingHouse();
		}
	}
}