using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Alms : AdventuresCardModel
	{
		public Alms()
		{
			this.Name = "Alms";
			this.Type = CardType.Event;
			this.Cost = 0;
		}

		public override void OnBuy(GameModel gameModel)
		{
			if (!gameModel.CurrentPlayer.Played.Any(c => c.Is(CardType.Treasure)) && !gameModel.CurrentPlayer.HasUsedAlms)
			{
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(Chooser.CardChoiceType.Gain, "Gain a card costing up to $4", gameModel.SupplyPiles.Where(p => p.Cost <= 4 && !p.CostsPotion && p.Count > 0));
				if (pile != null)
				{
					gameModel.CurrentPlayer.GainCard(pile);
				}
			}
			gameModel.CurrentPlayer.HasUsedAlms = true;
		}

		public override CardModel Clone()
		{
			return new Alms();
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
