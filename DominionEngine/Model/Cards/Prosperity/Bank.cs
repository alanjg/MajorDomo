using System;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class Bank : ProsperityCardModel
	{
		public Bank()
		{
			this.Type = CardType.Treasure;
			this.Name = "Bank";
			this.Cost = 7;
			this.Coins = 0;
		}

		public override bool AffectsTreasurePlayOrder
		{
			get
			{
				return true;
			}
		}
		public override void Play(GameModel gameModel)
		{
			int playedTreasures = gameModel.CurrentPlayer.Played.Count(card => card.Is(CardType.Treasure));
			gameModel.CurrentPlayer.AddActionCoin(playedTreasures);
		}

		public override CardModel Clone()
		{
			return new Bank();
		}
	}
}