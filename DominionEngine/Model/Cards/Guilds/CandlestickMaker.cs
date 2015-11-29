using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class CandlestickMaker : GuildsCardModel
	{
		public CandlestickMaker()
		{
			this.Name = "Candlestick Maker";
			this.Type = CardType.Action;
			this.Cost = 2;
			this.Actions = 1;
			this.Buys = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddCoinTokens(1);
		}

		public override CardModel Clone()
		{
			return new CandlestickMaker();
		}
	}
}
