using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Guide : AdventuresCardModel
	{
		public new const string Name = "Guide";
		public Guide()
		{
			base.Name = Guide.Name;
			this.Type = CardType.Action | CardType.Reserve;
			this.Cost = 3;
			
			this.Actions = 1;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override CardModel Clone()
		{
			return new Guide();
		}
	}
}
