using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class BanditCamp : DarkAgesCardModel
	{
		public BanditCamp()
		{
			this.Name = "Bandit Camp";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Cards = 1;
			this.Actions = 2;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Spoils));
		}

		public override CardModel Clone()
		{
			return new BanditCamp();
		}
	}
}