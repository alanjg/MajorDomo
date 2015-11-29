using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Port : AdventuresCardModel
	{
		public Port()
		{
			this.Name = "Port";
			this.Type = CardType.Action;
			this.Cost = 4;
			
			this.Actions = 2;
			this.Cards = 1;
		}

		public override void OnBuy(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Port));
		}

		public override CardModel Clone()
		{
			return new Port();
		}
	}
}
