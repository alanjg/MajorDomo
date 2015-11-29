using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Ratcatcher : AdventuresCardModel
	{
		public new const string Name = "Ratcatcher";
		public Ratcatcher()
		{
			base.Name = Ratcatcher.Name;
			this.Type = CardType.Action | CardType.Reserve;
			this.Cost = 2;

			this.Actions = 1;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromPlayed(this.ThisAsTrashTarget);
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override CardModel Clone()
		{
			return new Ratcatcher();
		}
	}
}
