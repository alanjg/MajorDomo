using System;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class City : ProsperityCardModel
	{
		public City()
		{
			this.Type = CardType.Action;
			this.Name = "City";
			this.Cost = 5;
			this.Actions = 2;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			int empty = gameModel.SupplyPiles.Count(pile => pile.Count == 0);
			if (empty >= 1)
			{
				gameModel.CurrentPlayer.Draw();
			}
			if (empty >= 2)
			{
				gameModel.CurrentPlayer.AddActionCoin(1);
				gameModel.CurrentPlayer.GainBuys(1);
			}
		}

		public override CardModel Clone()
		{
			return new City();
		}
	}
}