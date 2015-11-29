using System;

namespace Dominion.Model.Actions
{
	public class MerchantShip : SeasideCardModel
	{
		public MerchantShip()
		{
			this.Type = CardType.Action | CardType.Duration;
			this.Name = "Merchant Ship";
			this.Cost = 5;
			this.Coins = 2;
		}

		public override void Play(GameModel gameModel)
		{
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddActionCoin(2);
		}

		public override CardModel Clone()
		{
			return new MerchantShip();
		}
	}
}