using System;

namespace Dominion.Model.Actions
{
	public class Caravan : SeasideCardModel
	{
		public Caravan()
		{
			this.Type = CardType.Action | CardType.Duration;
			this.Name = "Caravan";
			this.Actions = 1;
			this.Cards = 1;
			this.Cost = 4;
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.Draw();
		}

		public override CardModel Clone()
		{
			return new Caravan();
		}
	}
}