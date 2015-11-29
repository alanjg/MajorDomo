using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Chancellor : BaseCardModel
	{
		public Chancellor()
		{
			this.Name = "Chancellor";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Coins = 2;
		}

		private static string[] choices = new string[] { "Discard", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Put your deck in the discard pile", "Do nothing" };
		public override void Play(GameModel gameModel)
		{
			int b = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.PutDeckInDiscard, "You may put your deck in the discard pile", choices, choiceDescriptions);
			if (b == 0)
			{
				gameModel.CurrentPlayer.PutDeckInDiscard();
			}
		}

		public override CardModel Clone()
		{
			return new Chancellor();
		}
	}
}