using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class RoyalCarriage : AdventuresCardModel
	{
		public new const string Name = "Royal Carriage";
		public RoyalCarriage()
		{
			base.Name = RoyalCarriage.Name;
			this.Type = CardType.Action | CardType.Reserve;
			this.Cost = 5;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromPlayed(this.ThisAsTrashTarget);
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override CardModel Clone()
		{
			return new RoyalCarriage();
		}
	}
}
