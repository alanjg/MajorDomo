using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Duplicate : AdventuresCardModel
	{
		public new const string Name = "Duplicate";
		public Duplicate()
		{
			base.Name = Duplicate.Name;
			this.Type = CardType.Action | CardType.Reserve;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromPlayed(this.ThisAsTrashTarget);
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override CardModel Clone()
		{
			return new Duplicate();
		}
	}
}
