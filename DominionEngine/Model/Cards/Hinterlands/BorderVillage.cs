using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class BorderVillage : HinterlandsCardModel
	{
		public BorderVillage()
		{
			this.Name = "Border Village";
			this.Type = CardType.Action;
			this.Cost = 6;
			this.Actions = 2;
			this.Cards = 1;
        }

        public override void OnGain(GameModel gameModel, Player player)
        {
			Pile choice = player.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing less than $" + gameModel.GetCost(this), gameModel.SupplyPiles.Where(pile => pile.Count > 0 && !pile.CostsPotion && gameModel.GetCost(pile) < gameModel.GetCost(this)));
			if (choice != null)
			{
				player.GainCard(choice);
			}
        }

		public override CardModel Clone()
		{
			return new BorderVillage();
		}
    }
}
