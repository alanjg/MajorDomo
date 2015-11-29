using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Transmogrify : AdventuresCardModel
	{
		public new const string Name = "Transmogrify";
		public Transmogrify()
		{
			base.Name = Transmogrify.Name;
			this.Type = CardType.Action | CardType.Reserve;
			this.Cost = 4;
			
			this.Actions = 1;
			
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromPlayed(this.ThisAsTrashTarget);
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override CardModel Clone()
		{
			return new Transmogrify();
		}
	}
}
