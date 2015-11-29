using System;

namespace Dominion.Model.Actions
{
	public class Wharf : SeasideCardModel
	{
		public Wharf()
		{
			this.Type = CardType.Action | CardType.Duration;
			this.Name = "Wharf";
			this.Cost = 5;
			this.Cards = 2;
			this.Buys = 1;
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.Draw(2);
			gameModel.CurrentPlayer.GainBuys(1);
		}

		public override CardModel Clone()
		{
			return new Wharf();
		}
	}
}