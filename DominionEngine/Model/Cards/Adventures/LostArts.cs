using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class LostArts : AdventuresCardModel
	{
		public LostArts()
		{
			this.Name = "LostArts";
			this.Type = CardType.Event;
			this.Cost = 6;
		}

		public override void OnBuy(GameModel gameModel)
		{
			Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(Chooser.CardChoiceType.LostArts, "Move your action token to an Action Supply pile.", gameModel.SupplyPiles.Where(p => p.Card.Is(CardType.Action)));
			if (pile != null)
			{
				gameModel.CurrentPlayer.ActionPile = pile;
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
			return new LostArts();
		}
	}
}
