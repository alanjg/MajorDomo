using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Embargo : SeasideCardModel
	{
		public Embargo()
		{
			this.Type = CardType.Action;
			this.Name = "Embargo";
			this.Cost = 2;
			this.Coins = 2;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.Trash(this.ThisAsTrashTarget);
			Pile chosenPile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Embargo, "Embargo a pile", gameModel.SupplyPiles);
			if (chosenPile != null)
			{
				chosenPile.EmbargoCount++;
				gameModel.TextLog.WriteLine(gameModel.CurrentPlayer.Name + " embargos " + chosenPile.Name);
			}
		}

		public override CardModel Clone()
		{
			return new Embargo();
		}
	}
}