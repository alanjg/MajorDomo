using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Fortress : DarkAgesCardModel
	{
		public Fortress()
		{
			this.Name = "Fortress";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Actions = 2;
			this.Cards = 1;
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			if (gameModel.Trash.Remove(this.ThisAsTrashTarget))
			{
				owner.PutInHand(this.ThisAsTrashTarget);
			}
		}

		public override CardModel Clone()
		{
			return new Fortress();
		}
	}
}