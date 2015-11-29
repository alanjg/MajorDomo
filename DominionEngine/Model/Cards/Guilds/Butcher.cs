using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Butcher : GuildsCardModel
	{
		public Butcher()
		{
			this.Name = "Butcher";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddCoinTokens(2);

			CardModel card = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashForButcher, "You may trash a card for Butcher", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (card != null)
			{
				gameModel.CurrentPlayer.Trash(card);
				int max = Math.Min(11, gameModel.CurrentPlayer.CoinTokens);
				string[] choices = new string[max+1];
				for (int i = 0; i <= max; i++)
				{
					choices[i] = i.ToString();
				}
				int pay = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Butcher, "Pay any number of coin tokens for Butcher", choices, choices);
				gameModel.CurrentPlayer.AddCoinTokens(-pay);
				int newCardMax = gameModel.GetCost(card) + pay;
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to $" + newCardMax, gameModel.SupplyPiles.Where(p => !p.CostsPotion && p.GetCost() <= newCardMax));
				if (pile != null)
				{
					gameModel.CurrentPlayer.GainCard(pile);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Butcher();
		}
	}
}
