using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class CouncilRoom : BaseCardModel
	{
		public CouncilRoom()
		{
			this.Name = "Council Room";
			this.Type = CardType.Action;
			this.Cards = 4;
			this.Buys = 1;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			foreach (Player player in gameModel.Players)
			{
				if (player != gameModel.CurrentPlayer)
				{
					player.Draw();
				}
			}
		}

		public override CardModel Clone()
		{
			return new CouncilRoom();
		}
	}
}
