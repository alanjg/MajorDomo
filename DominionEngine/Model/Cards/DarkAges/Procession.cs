using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Procession : DarkAgesCardModel
	{
		public Procession()
		{
			this.Name = "Procession";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.Procession, "You may play an action card from your hand twice.", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Action)));
			if (choice != null)
			{
				using (choice.ForceMultipleCardPlayChoice())
				{
					gameModel.CurrentPlayer.Play(choice, false, true);
					gameModel.CurrentPlayer.Play(choice, false, false, " a second time");
				}
				if (gameModel.CurrentPlayer.Played.Contains(choice))
				{
					gameModel.CurrentPlayer.Trash(choice);
				}
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain an action card costing $" + (gameModel.GetCost(choice) + 1).ToString(), gameModel.SupplyPiles.Where(p => p.Count > 0 && p.GetCost() == gameModel.GetCost(choice) + 1 && p.CostsPotion == choice.CostsPotion && p.Card.Is(CardType.Action)));
				if (pile != null)
				{
					gameModel.CurrentPlayer.GainCard(pile);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Procession();
		}
	}
}