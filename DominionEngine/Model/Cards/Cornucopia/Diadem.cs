using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Diadem : CornucopiaCardModel
	{
		public Diadem()
		{
			this.Name = "Diadem";
			this.Cost = 0;
			this.Type = CardType.Treasure | CardType.Prize;
			this.Coins = 2;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddActionCoin(gameModel.CurrentPlayer.Actions);
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Diadem();
		}
	}
}
