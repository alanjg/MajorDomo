using System;

namespace Dominion.Model.Actions
{
	public class Bridge : IntrigueCardModel
	{
		public Bridge()
		{
			this.Type = CardType.Action;
			this.Name = "Bridge";
			this.Cost = 4;
			this.Buys = 1;
			this.Coins = 1;
		}

		private class BridgeCardModifier : CardModifier
		{
			public override int GetCost(CardModel cardModel, int cost)
			{
				return Math.Max(cost - 1, 0);
			}
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.AddCardModifier(new BridgeCardModifier());
		}

		public override CardModel Clone()
		{
			return new Bridge();
		}
	}
}
