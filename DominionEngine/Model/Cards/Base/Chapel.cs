using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Chapel : BaseCardModel
	{
		public Chapel()
		{
			this.Name = "Chapel";
			this.Cost = 2;
			this.Type = CardType.Action;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> trashed = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.TrashFromHand, "Trash up to 4 cards from your hand", ChoiceSource.FromHand, 0, Math.Min(4, gameModel.CurrentPlayer.Hand.Count), gameModel.CurrentPlayer.Hand);
			IList<CardModel> cards = trashed.ToList();
			for(int i = cards.Count - 1; i >= 0; i--)
			{
				gameModel.CurrentPlayer.Trash(cards[i]);
			}
		}

		public override CardModel Clone()
		{
			return new Chapel();
		}
	}
}
