using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class BagOfGold : CornucopiaCardModel
	{
		public BagOfGold()
		{
			this.Name = "Bag of Gold";
			this.Type = CardType.Action | CardType.Prize;
			this.Cost = 0;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Gold), GainLocation.TopOfDeck);
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new BagOfGold();
		}
	}
}
