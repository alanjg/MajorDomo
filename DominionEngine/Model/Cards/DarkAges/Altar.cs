using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Altar : DarkAgesCardModel
	{
		public Altar()
		{
			this.Name = "Altar";
			this.Type = CardType.Action;
			this.Cost = 6;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashFromHand, "Trash a card from your hand.", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (choice != null)
			{
				gameModel.CurrentPlayer.Trash(choice);
			}
			IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(p => p.Count > 0 && !p.CostsPotion && p.GetCost() <= 5);
			if (piles.Any())
			{
				Pile toGain = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to $5", piles);
				gameModel.CurrentPlayer.GainCard(toGain);
			}
		}

		public override CardModel Clone()
		{
			return new Altar();
		}
	}
}