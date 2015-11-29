using System;
using System.Linq;
using System.Net;

namespace Dominion.Model.Actions
{
	public class GrandMarket : ProsperityCardModel
	{
		public GrandMarket()
		{
			this.Type = CardType.Action;
			this.Name = "Grand Market";
			this.Cost = 6;
			this.Actions = 1;
			this.Cards = 1;
			this.Coins = 2;
			this.Buys = 1;
		}

		public override bool AffectsTreasureBuyOrder
		{
			get
			{
				return true;
			}
		}

		public override bool CanBuy(Player player)
		{
			return !player.Played.Any(card => card is Copper);
		}

		public override CardModel Clone()
		{
			return new GrandMarket();
		}
	}
}