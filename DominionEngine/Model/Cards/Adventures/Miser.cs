using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Miser : AdventuresCardModel
	{
		public Miser()
		{
			this.Name = "Miser";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		static string[] choices = new string[] { "Copper", "Coin" };
		static string[] choiceDescriptions = new string[] { "Put a Copper from your hand onto your Tavern mat", "+$1 per Copper on your Tavern mat" };
		public override void Play(GameModel gameModel)
		{
			int coppers = gameModel.CurrentPlayer.Tavern.Count(c => c is Copper);
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(Chooser.EffectChoiceType.Miser, "Put a Copper from your hand onto your Tavern mat, or +$" + coppers, choices, choiceDescriptions);
			if(choice == 0)
			{
				CardModel copper = gameModel.CurrentPlayer.Hand.FirstOrDefault(c => c is Copper);
				if (copper != null)
				{
					gameModel.CurrentPlayer.Hand.Remove(copper);
					gameModel.CurrentPlayer.Tavern.Add(copper);
				}
			}
			else
			{
				gameModel.CurrentPlayer.AddActionCoin(coppers);
			}
		}
		public override CardModel Clone()
		{
			return new Miser();
		}
	}
}
