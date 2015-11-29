using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Baker : GuildsCardModel
	{
		public Baker()
		{
			this.Name = "Baker";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 1;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddCoinTokens(1);
		}

		public override CardModel Clone()
		{
			return new Baker();
		}
	}
}
