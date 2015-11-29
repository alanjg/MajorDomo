using System;

namespace Dominion.Model.Actions
{
	public class Monument : ProsperityCardModel
	{
		public Monument()
		{
			this.Type = CardType.Action;
			this.Name = "Monument";
			this.Cost = 4;
			this.Coins = 2;
		}
		
		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddVPChips(1);
		}

		public override CardModel Clone()
		{
			return new Monument();
		}
	}
}