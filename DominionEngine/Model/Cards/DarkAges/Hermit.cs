using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Hermit : DarkAgesCardModel
	{
		public Hermit()
		{
			this.Name = "Hermit";
			this.Type = CardType.Action;
			this.Cost = 3;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashFromHand, "You may trash a card from your discard pile or hand that is not a Treasure", Chooser.ChoiceSource.FromHand | Chooser.ChoiceSource.None, gameModel.CurrentPlayer.Hand.Union(gameModel.CurrentPlayer.Discard).Where(c => !c.Is(CardType.Treasure)));
			if (choice != null)
			{
				gameModel.CurrentPlayer.Trash(choice);
			}
			IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(p => p.GetCost() <= 3 && !p.CostsPotion && p.Count > 0);
			Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to $3", piles);
			if (pile != null)
			{
				gameModel.CurrentPlayer.GainCard(pile);
			}
		}

		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			if (gameModel.CurrentPlayer.Bought.Count == 0)
			{
				gameModel.CurrentPlayer.Trash(this.ThisAsTrashTarget);
				gameModel.CurrentPlayer.GainCard(typeof(Madman));
			}
		}

		public override CardModel Clone()
		{
			return new Hermit();
		}
	}
}