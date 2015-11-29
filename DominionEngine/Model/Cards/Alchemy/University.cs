using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class University : AlchemyCardModel
	{
		public University()
		{
			this.Name = "University";
			this.Type = CardType.Action;
			this.Actions = 2;
			this.Cost = 2;
			this.CostsPotion = true;
		}

		public override void Play(GameModel gameModel)
		{
			Pile choice = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOnePile(CardChoiceType.Gain, "You may gain an action costing up to $5", gameModel.SupplyPiles.Where(
																		pile => gameModel.GetCost(pile) <= 5 && pile.Count > 0 && !pile.CostsPotion && pile.Card.Is(CardType.Action)));
			if (choice != null)
			{
				gameModel.CurrentPlayer.GainCard(choice);
			}
		}

		public override CardModel Clone()
		{
			return new University();
		}
	}
}
