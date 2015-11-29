using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class CoinOfTheRealm : AdventuresCardModel
	{
		public CoinOfTheRealm()
		{
			this.Name = "Coin Of The Realm";
			this.Type = CardType.Treasure | CardType.Reserve;
			this.Cost = 2;
			this.Coins = 1;
		}

		public override void Play(GameModel gameModel)
		{
			this.ThisAsTrashTarget.OnRemovedFromPlay(gameModel);
			gameModel.CurrentPlayer.RemoveFromPlayed(this.ThisAsTrashTarget);
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override void Call(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainActions(2);
		}

		public override CardModel Clone()
		{
			return new CoinOfTheRealm();
		}
	}
}
