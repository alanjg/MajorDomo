using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class WineMerchant : AdventuresCardModel
	{
		public new const string Name = "Wine Merchant";
		public WineMerchant()
		{
			base.Name = WineMerchant.Name;
			this.Type = CardType.Action | CardType.Reserve;
			this.Cost = 5;
			
			this.Buys = 1;
			this.Coins = 4;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromPlayed(this.ThisAsTrashTarget);
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override CardModel Clone()
		{
			return new WineMerchant();
		}
	}
}
