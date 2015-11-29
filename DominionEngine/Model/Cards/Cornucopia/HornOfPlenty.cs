using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class HornOfPlenty : CornucopiaCardModel
	{
		public HornOfPlenty()
		{
			this.Name = "Horn of Plenty";
			this.Type = CardType.Treasure;
			this.Cost = 5;
		}

		public override bool AffectsTreasurePlayOrder
		{
			get
			{
				return true;
			}
		}

		public override void Play(GameModel gameModel)
		{
			int value = gameModel.CurrentPlayer.Played.Select(card => card.Name).Distinct().Count();
			Pile choice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.GainForHornOfPlenty, "Gain a card costing up to $" + value.ToString(), gameModel.SupplyPiles.Where(pile => gameModel.GetCost(pile) <= value && !pile.CostsPotion && pile.Count > 0));
			if (choice != null)
			{
				CardModel card = gameModel.CurrentPlayer.GainCard(choice);
				if (card != null && card.Is(CardType.Victory))
				{
					gameModel.CurrentPlayer.Trash(this);
				}
			}
		}

		public override CardModel Clone()
		{
			return new HornOfPlenty();
		}
	}
}
