using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class KingsCourt : ProsperityCardModel
	{
		public KingsCourt()
		{
			this.Type = CardType.Action;
			this.Cost = 7;
			this.Name = "King's Court";
		}

		public override void Play(GameModel gameModel)
		{
			CardModel card = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.KingsCourt, "Choose an action to play 3 times", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Action)));
			if (card != null)
			{
				card.DurationPlayMultiplier = 3;
				using (card.ForceMultipleCardPlayChoice())
				{
					gameModel.CurrentPlayer.Play(card, false, true);
					gameModel.CurrentPlayer.Play(card, false, false, " a second time");
					gameModel.CurrentPlayer.Play(card, false, false, " a third time");
				}
			}
		}

		public override CardModel Clone()
		{
			return new KingsCourt();
		}
	}
}