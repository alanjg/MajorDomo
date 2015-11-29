using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Peasant : AdventuresCardModel
	{
		public Peasant()
		{
			this.Name = "Peasant";
			this.Type = CardType.Action | CardType.Traveller;
			this.Cost = 2;
			
			this.Coins = 1;
			this.Buys = 1;
		}

		private static string[] exchangeChoices = new string[] { "Exchange", "Keep" };

		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ExchangePeasantForSoldier, "You may exchange Peasant for a Soldier", exchangeChoices, exchangeChoices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.Played.Remove(this);
				gameModel.PileMap[typeof(Peasant)].PutCardOnPile(this);
				gameModel.CurrentPlayer.GainCard(typeof(Soldier));
			}
		}

		public override CardModel Clone()
		{
			return new Peasant();
		}
	}
}
