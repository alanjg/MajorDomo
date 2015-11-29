using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public abstract class GuildsCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Guilds"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Guilds; }
		}

		protected int Overpay(GameModel gameModel, EffectChoiceType effectChoiceType, string choiceDescription)
		{
			int max = gameModel.CurrentPlayer.Coin;
			if (max > 0)
			{
				string[] choices = new string[max + 1];
				for (int i = 0; i <= max; i++)
				{
					choices[i] = i.ToString();
				}
				int result = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(effectChoiceType, choiceDescription, choices, choices);
				gameModel.CurrentPlayer.AddActionCoin(-result);
				return result;
			}			
			return 0;
		}
	}
}