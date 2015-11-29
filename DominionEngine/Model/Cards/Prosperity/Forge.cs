using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Forge : ProsperityCardModel
	{
		public Forge()
		{
			this.Type = CardType.Action;
			this.Name = "Forge";
			this.Cost = 7;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> trashedCards = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.TrashForForge, "Choose cards to trash", ChoiceSource.FromHand, 0, gameModel.CurrentPlayer.Hand.Count, gameModel.CurrentPlayer.Hand);
			int cost = 0;
			foreach (CardModel card in trashedCards.ToList())
			{
				gameModel.CurrentPlayer.Trash(card);
				cost += gameModel.GetCost(card);
			}
			IEnumerable<Pile> piles = from pile in gameModel.SupplyPiles where gameModel.GetCost(pile) == cost && pile.Count > 0 && !pile.CostsPotion select pile;
			Pile newChoice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing " + cost, piles);
			if (newChoice != null)
			{
				gameModel.CurrentPlayer.GainCard(newChoice);
			}							
		}

		public override CardModel Clone()
		{
			return new Forge();
		}
	}
}