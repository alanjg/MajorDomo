using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class HuntingGrounds : DarkAgesCardModel
	{
		public HuntingGrounds()
		{
			this.Name = "Hunting Grounds";
			this.Type = CardType.Action;
			this.Cost = 6;
			this.Cards = 4;
		}

		private static string[] choices = new string[] { "Duchy", "Estates" };
		public override void OnTrash(GameModel gameModel, Player owner)
		{
			int choice = owner.Chooser.ChooseOneEffect(EffectChoiceType.GainForHuntingGrounds, "Gain a Duchy or 3 Estates", choices, choices);
			if (choice == 0)
			{
				owner.GainCard(typeof(Duchy));
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					owner.GainCard(typeof(Estate));
				}
			}
		}

		public override CardModel Clone()
		{
			return new HuntingGrounds();
		}
	}
}