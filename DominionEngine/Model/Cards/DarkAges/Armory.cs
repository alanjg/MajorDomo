using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Armory : DarkAgesCardModel
	{
		public Armory()
		{
			this.Name = "Armory";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(p => p.Count > 0 && !p.CostsPotion && p.GetCost() <= 4);
			if (piles.Count() > 0)
			{
				Pile choice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.GainOnTopOfDeck, "Gain a card costing up to $4", piles);
				if (choice != null)
				{
					gameModel.CurrentPlayer.GainCard(choice, GainLocation.TopOfDeck);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Armory();
		}
	}
}