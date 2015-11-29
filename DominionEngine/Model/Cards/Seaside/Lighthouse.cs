using System;

namespace Dominion.Model.Actions
{
	public class Lighthouse : SeasideCardModel
	{
		public new const string Name = "Lighthouse";
		public Lighthouse()
		{
			this.Type = CardType.Action | CardType.Duration;
			base.Name = Lighthouse.Name;
			this.Cost = 2;
			this.Actions = 1;
			this.Coins = 1;
		}

		public override void Play(GameModel gameModel)
		{
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddActionCoin(1);
		}

		public override CardModel Clone()
		{
			return new Lighthouse();
		}
	}
}