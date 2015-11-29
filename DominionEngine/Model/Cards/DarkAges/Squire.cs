using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Squire : DarkAgesCardModel
	{
		public Squire()
		{
			this.Name = "Squire";
			this.Type = CardType.Action;
			this.Cost = 2;
			this.Coins = 1;
		}

		private static string[] choices = new string[] { "Actions", "Buys", "Silver" };
		private static string[] choiceDescriptions = new string[] { "+2 Actions", "+2 Buys", "Gain a Silver" };
		public override void Play(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Squire, "Choose one", choices, choiceDescriptions);
			switch (choice)
			{
				case 0:
					gameModel.CurrentPlayer.GainActions(2);
					break;
				case 1:
					gameModel.CurrentPlayer.GainBuys(2);
					break;
				case 2:
					gameModel.CurrentPlayer.GainCard(typeof(Silver));
					break;
			}
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			Pile pile = owner.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain an Attack card", gameModel.SupplyPiles.Where(p => p.Count > 0 && p.Card.Is(CardType.Attack)));
			if (pile != null)
			{
				owner.GainCard(pile);
			}
		}

		public override CardModel Clone()
		{
			return new Squire();
		}
	}
}