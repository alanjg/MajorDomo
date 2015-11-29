using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Training : AdventuresCardModel
	{
		public Training()
		{
			this.Name = "Training";
			this.Type = CardType.Event;
			this.Cost = 6;
		}

		public override void OnBuy(GameModel gameModel)
		{
			Pile targetPile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.ActionTokenPile, "Move your +$1 token to an Action Supply pile", gameModel.SupplyPiles.Where(p => p.Card.Is(CardType.Action)));
			if (targetPile != null)
			{
				gameModel.CurrentPlayer.CoinPile = targetPile;
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
			return new Training();
		}
	}
}
