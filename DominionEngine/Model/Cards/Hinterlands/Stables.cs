using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Stables : HinterlandsCardModel
	{
		public Stables()
		{
			this.Name = "Stables";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		private static string[] discard = new string[] { "Yes", "No" };
		
		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> treasures = gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Treasure));
			if (treasures.Any())
			{
				int discardATreasure = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardForStables, "Discard a treasure?", discard, discard);
				if (discardATreasure == 0)
				{
					CardModel chosenTreasure = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.Discard, "Discard a treasure", ChoiceSource.FromHand, treasures);
					gameModel.CurrentPlayer.DiscardCard(chosenTreasure);
							
					gameModel.CurrentPlayer.GainActions(1);
					gameModel.CurrentPlayer.Draw(3);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Stables();
		}
	}
}
