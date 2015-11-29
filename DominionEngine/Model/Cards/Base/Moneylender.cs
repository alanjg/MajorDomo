using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Moneylender : BaseCardModel
	{
		public Moneylender()
		{
			this.Name = "Moneylender";
			this.Type = CardType.Action;
			this.Cost = 4;
		}
		public override void Play(GameModel gameModel)
		{
			CardModel copper = gameModel.CurrentPlayer.Hand.FirstOrDefault(card => card is Copper);
			if (copper != null)
			{
				gameModel.CurrentPlayer.Trash(copper);
				gameModel.CurrentPlayer.AddActionCoin(3);
			}
		}

		public override CardModel Clone()
		{
			return new Moneylender();
		}
	}
}
