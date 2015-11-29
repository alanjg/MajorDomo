using System;

namespace Dominion.Model.Actions
{
	public class Coppersmith : IntrigueCardModel
	{
		public Coppersmith()
		{
			this.Type = CardType.Action;
			this.Name = "Coppersmith";
			this.Cost = 4;
		}

		private class CoppersmithCardModifier : CardModifier
		{
			public override int GetCoins(CardModel cardModel, int coins)
			{
				if (cardModel is Copper)
				{
					return coins + 1;
				}
				return coins;
			}
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.AddCardModifier(new CoppersmithCardModifier());
		}

		public override CardModel Clone()
		{
			return new Coppersmith();
		}
	}
}
