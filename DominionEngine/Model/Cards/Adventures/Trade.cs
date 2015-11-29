using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Trade : AdventuresCardModel
	{
		public Trade()
		{
			this.Name = "Trade";
			this.Type = CardType.Event;
			this.Cost = 5;
		}

		public override void OnBuy(GameModel gameModel)
		{
			IEnumerable<CardModel> trashed = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.TrashFromHand, "Trash up to 2 cards from your hand", ChoiceSource.FromHand, 0, Math.Min(2, gameModel.CurrentPlayer.Hand.Count), gameModel.CurrentPlayer.Hand);
			IList<CardModel> cards = trashed.ToList();
			for (int i = cards.Count - 1; i >= 0; i--)
			{
				gameModel.CurrentPlayer.Trash(cards[i]);
				gameModel.CurrentPlayer.GainCard(typeof(Silver));
			}
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}

		public override CardModel Clone()
		{
			return new Trade();
		}
	}
}
