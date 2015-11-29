using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Inheritance : AdventuresCardModel
	{
		public Inheritance()
		{
			this.Name = "Inheritance";
			this.Type = CardType.Event;
			this.Cost = 7;			
		}

		public override void OnBuy(GameModel gameModel)
		{
			if (!gameModel.CurrentPlayer.HasUsedInheritance)
			{
				gameModel.CurrentPlayer.HasUsedInheritance = true;
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(Chooser.CardChoiceType.Inheritance, "Set aside a non-Victory Action card costing up to $4", gameModel.SupplyPiles.Where(p => p.Count > 0 && !p.CostsPotion && p.Cost <= 4 && p.TopCard.Is(CardType.Action) && !p.TopCard.Is(CardType.Victory)));
				if (pile != null)
				{
					pile.DrawCard();
					gameModel.CurrentPlayer.EstatePile = pile;
				}
			}
		}

		public override CardModel Clone()
		{
			return new Inheritance();
		}
	}
}
