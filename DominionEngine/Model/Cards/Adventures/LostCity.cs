using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class LostCity : AdventuresCardModel
	{
		public LostCity()
		{
			this.Name = "Lost City";
			this.Type = CardType.Action;
			this.Cost = 5;
			
			this.Actions = 2;
			this.Cards = 2;
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			foreach(Player otherPlayer in gameModel.Players)
			{
				if(otherPlayer != player)
				{
					otherPlayer.Draw(1);
				}
			}
		}

		public override CardModel Clone()
		{
			return new LostCity();
		}
	}
}
