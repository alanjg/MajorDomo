using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Raze : AdventuresCardModel
	{
		public Raze()
		{
			this.Name = "Raze";
			this.Type = CardType.Action;
			this.Cost = 2;
			
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> sources = gameModel.CurrentPlayer.Hand.Union(new CardModel[] {this});
			CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(Chooser.CardChoiceType.Raze, "Trash this or a card from your hand", Chooser.ChoiceSource.FromHand, sources);
			if (card != null)
			{
				gameModel.CurrentPlayer.Trash(card.ThisAsTrashTarget);
				int cost = gameModel.GetCost(card);
				IList<CardModel> lookAt = gameModel.CurrentPlayer.DrawCards(cost);
				CardModel put = gameModel.CurrentPlayer.Chooser.ChooseOneCard(Chooser.CardChoiceType.PutInHand, "Put one into your hand and discard the rest", Chooser.ChoiceSource.None, lookAt);
				if(put != null)
				{
					lookAt.Remove(put);
					gameModel.CurrentPlayer.PutInHand(put);
					foreach (CardModel discard in lookAt)
					{
						gameModel.CurrentPlayer.DiscardCard(discard);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Raze();
		}
	}
}
