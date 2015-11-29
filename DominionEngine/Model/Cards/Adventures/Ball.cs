using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Ball : AdventuresCardModel
	{
		public Ball()
		{
			this.Name = "Ball";
			this.Type = CardType.Event;
			this.Cost = 5;
		}

		public override void OnBuy(GameModel gameModel)
		{
			gameModel.CurrentPlayer.HasMinusOneCoinToken = true;
			for (int i = 0; i < 4; i++)
			{
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(Chooser.CardChoiceType.Gain, "Gain a card costing up to $4", gameModel.SupplyPiles.Where(p => p.Cost <= 4 && !p.CostsPotion && p.Count > 0));
				if(pile != null)
				{
					gameModel.CurrentPlayer.GainCard(pile);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Ball();
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}
	}
}
