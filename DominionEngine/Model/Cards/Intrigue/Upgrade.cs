using System;
using System.Linq;
using System.Net;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Upgrade : IntrigueCardModel
	{
		public Upgrade()
		{
			this.Type = CardType.Action;
			this.Name = "Upgrade";
			this.Actions = 1;
			this.Cost = 5;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel trashedCard = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForUpgrade, "Trash a card", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (trashedCard != null)
			{
				gameModel.CurrentPlayer.Trash(trashedCard);
				Pile newChoice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing $" + (gameModel.GetCost(trashedCard) + 1).ToString(), 
					gameModel.SupplyPiles.Where(pile => gameModel.GetCost(pile) == gameModel.GetCost(trashedCard) + 1 && (trashedCard.CostsPotion == pile.CostsPotion) && pile.Count > 0));

				if (newChoice != null)
				{
					gameModel.CurrentPlayer.GainCard(newChoice);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Upgrade();
		}
	}
}
