using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class ThroneRoom : BaseCardModel
	{
		public ThroneRoom()
		{
			this.Name = "Throne Room";
			this.Type = CardType.Action;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.ThroneRoom, "Choose an action to play twice", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Action)));
			if (card != null)
			{
				card.DurationPlayMultiplier = 2;
				using (card.ForceMultipleCardPlayChoice())
				{
					gameModel.CurrentPlayer.Play(card, false, true);
					gameModel.CurrentPlayer.Play(card, false, false, " a second time");
				}
			}
		}

		public override CardModel Clone()
		{
			return new ThroneRoom();
		}
	}
}
