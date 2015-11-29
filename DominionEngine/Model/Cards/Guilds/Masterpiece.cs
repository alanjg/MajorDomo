using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Masterpiece : GuildsCardModel
	{
		public Masterpiece()
		{
			this.Name = "Masterpiece";
			this.Type = CardType.Treasure;
			this.Cost = 3;
			this.Coins = 1;
		}

		public override void OnBuy(GameModel gameModel)
		{
			int amount = this.Overpay(gameModel, EffectChoiceType.MasterpiecePay, "You may overpay for Masterpiece");
			for (int i = 0; i < amount; i++)
			{
				gameModel.CurrentPlayer.GainCard(typeof(Silver));
			}
		}

		public override CardModel Clone()
		{
			return new Masterpiece();
		}
	}
}
