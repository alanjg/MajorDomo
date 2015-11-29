using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Remodel : BaseCardModel
	{
		public Remodel()
		{
			this.Name = "Remodel";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel trashedCard = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForRemodel, "Choose a card to Remodel", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (trashedCard != null)
			{
				gameModel.CurrentPlayer.Trash(trashedCard);
				Pile newChoice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Choose a card to gain", gameModel.SupplyPiles.Where(pile =>
																gameModel.GetCost(pile) <= gameModel.GetCost(trashedCard) + 2 && pile.Count > 0 &&
																(trashedCard.CostsPotion || !pile.CostsPotion)));
				if (newChoice != null)
				{
					gameModel.CurrentPlayer.GainCard(newChoice);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Remodel();
		}
	}
}
