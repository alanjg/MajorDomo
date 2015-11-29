using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Hireling : AdventuresCardModel
	{
		public new const string Name = "Hireling";
		public Hireling()
		{
			base.Name = Hireling.Name;
			this.Type = CardType.Action | CardType.Duration;
			this.Cost = 6;
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.Draw(1);
		}

		public override CardModel Clone()
		{
			return new Hireling();
		}
	}
}
