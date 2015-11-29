using System;

namespace Dominion.Model.Actions
{
	public class Quarry : ProsperityCardModel
	{
		public Quarry()
		{
			this.Type = CardType.Treasure;
			this.Name = "Quarry";
			this.Cost = 4;
			this.Coins = 1;
		}

		private class QuarryCardModifier : CardModifier
		{
			public override int GetCost(CardModel cardModel, int cost)
			{
				if (cardModel.Is(CardType.Action))
				{
					return Math.Max(cost - 2, 0);
				}
				else
				{
					return cost;
				}
			}
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.AddCardModifier(new QuarryCardModifier());
		}

		public override CardModel Clone()
		{
			return new Quarry();
		}
	}
}