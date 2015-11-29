using System;

namespace Dominion.Model.Actions
{
	public class FishingVillage : SeasideCardModel
	{
		public FishingVillage()
		{
			this.Type = CardType.Action | CardType.Duration;
			this.Name = "Fishing Village";
			this.Cost = 3;
			this.Actions = 2;
			this.Coins = 1;
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddActionCoin(1);
			gameModel.CurrentPlayer.GainActions(1);
		}

		public override CardModel Clone()
		{
			return new FishingVillage();
		}
	}
}