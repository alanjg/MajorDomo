using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class MerchantGuild : GuildsCardModel
	{
		public MerchantGuild()
		{
			this.Name = "Merchant Guild";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Buys = 1;
			this.Coins = 1;
		}

		public override CardModel Clone()
		{
			return new MerchantGuild();
		}
	}
}
