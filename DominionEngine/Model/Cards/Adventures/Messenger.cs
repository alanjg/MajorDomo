using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Messenger : AdventuresCardModel
	{
		public Messenger()
		{
			this.Name = "Messenger";
			this.Type = CardType.Action;
			this.Cost = 4;
			
			this.Buys = 1;
			this.Coins = 2;
		}
		
		private static string[] choices = new string[] { "Discard", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Put your deck in the discard pile", "Do nothing" };
		
		public override void Play(GameModel gameModel)
		{
			int b = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.PutDeckInDiscard, "You may put your deck in the discard pile", choices, choiceDescriptions);
			if (b == 0)
			{
				gameModel.CurrentPlayer.PutDeckInDiscard();
			}
		}

		public override void OnBuy(GameModel gameModel)
		{
			if(gameModel.CurrentPlayer.Bought.Count == 0)
			{
				IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(pile => pile.Count > 0 && gameModel.GetCost(pile) <= 4 && !pile.CostsPotion);
				Pile chosenPile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to $4", piles);
				if (chosenPile != null)
				{
					gameModel.CurrentPlayer.GainCard(chosenPile);
					foreach(Player otherPlayer in gameModel.Players)
					{
						if(otherPlayer != gameModel.CurrentPlayer)
						{
							otherPlayer.GainCard(chosenPile);
						}
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Messenger();
		}
	}
}
