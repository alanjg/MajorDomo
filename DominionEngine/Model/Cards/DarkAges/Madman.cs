using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Madman : DarkAgesCardModel
	{
		public Madman()
		{
			this.Name = "Madman";
			this.Type = CardType.Action;
			this.Cost = 0;
			this.Actions = 2;
		}

		public override void Play(GameModel gameModel)
		{
			if (gameModel.CurrentPlayer.Played.Contains(this))
			{
				gameModel.CurrentPlayer.RemoveFromPlayed(this);
				gameModel.CurrentPlayer.Draw(gameModel.CurrentPlayer.Hand.Count);
			}			
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Madman();
		}
	}
}