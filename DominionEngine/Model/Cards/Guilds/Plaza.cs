using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Plaza : GuildsCardModel
	{
		public Plaza()
		{
			this.Name = "Plaza";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Actions = 2;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel discard = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.Plaza, "You may discard a treasure", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Treasure)));
			if (discard != null)
			{
				gameModel.CurrentPlayer.DiscardCard(discard);
				gameModel.CurrentPlayer.AddCoinTokens(1);
			}
		}

		public override CardModel Clone()
		{
			return new Plaza();
		}
	}
}
