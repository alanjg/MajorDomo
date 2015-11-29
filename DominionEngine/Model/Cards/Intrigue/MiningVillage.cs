using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class MiningVillage : IntrigueCardModel
	{
		public MiningVillage()
		{
			this.Name = "Mining Village";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Actions = 2;
			this.Cards = 1;
		}

		private static string[] choices = new string[] { "Trash", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Trash Mining Village", "Do nothing" };
		public override void Play(GameModel gameModel)
		{
			if (!gameModel.Trash.Contains(this.ThisAsTrashTarget))
			{
				Player currentPlayer = gameModel.CurrentPlayer;

				int trash = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.TrashMiningVillage, "You may trash Mining Village for +2$", choices, choiceDescriptions);
				if (trash == 0)
				{
					currentPlayer.Trash(this.ThisAsTrashTarget);
					currentPlayer.AddActionCoin(2);
				}
			}
		}

		public override CardModel Clone()
		{
			return new MiningVillage();
		}
	}
}
