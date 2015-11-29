using System;

namespace Dominion.Model.Actions
{
	public class Outpost : SeasideCardModel
	{
		public Outpost()
		{
			this.Type = CardType.Action | CardType.Duration;
			this.Name = "Outpost";
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainOutpostTurn();
		}

		public override CardModel Clone()
		{
			return new Outpost();
		}
	}
}