using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Page : AdventuresCardModel
	{
		public Page()
		{
			this.Name = "Page";
			this.Type = CardType.Action | CardType.Traveller;
			this.Cost = 2;
			
			this.Actions = 1;
			this.Cards = 1;
		}
		private static string[] exchangeChoices = new string[] { "Exchange", "Keep" };
		
		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ExchangePageForTreasureHunter, "You may exchange Page for a Treasure Hunter", exchangeChoices, exchangeChoices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.Played.Remove(this);
				gameModel.PileMap[typeof(Page)].PutCardOnPile(this);
				gameModel.CurrentPlayer.GainCard(typeof(TreasureHunter));
			}
		}

		public override CardModel Clone()
		{
			return new Page();
		}
	}
}
