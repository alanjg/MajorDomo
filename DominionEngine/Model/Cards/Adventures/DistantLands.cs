using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class DistantLands : AdventuresCardModel
	{
		public DistantLands()
		{
			this.Name = "Distant Lands";
			this.Type = CardType.Action | CardType.Reserve | CardType.Victory;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromPlayed(this.ThisAsTrashTarget);
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override int GetVictoryPoints(Player player)
		{
			if (player.Tavern.Contains(this.ThisAsTrashTarget))
			{
				return 4;
			}
			else
			{
				return 0;
			}
		}

		public override CardModel Clone()
		{
			return new DistantLands();
		}
	}
}
