using System;
using System.Net;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class Cache : HinterlandsCardModel
	{
		public Cache()
		{
			this.Name = "Cache";
			this.Type = CardType.Treasure;
			this.Cost = 5;
			this.Coins = 3;
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			player.GainCard(typeof(Copper));
			player.GainCard(typeof(Copper));
		}

		public override CardModel Clone()
		{
			return new Cache();
		}
	}
}
