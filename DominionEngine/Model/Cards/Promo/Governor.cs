using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Governor : PromoCardModel
	{
		public Governor()
		{
			this.Type = CardType.Action;
			this.Name = "Governor";
			this.Cost = 5;
			this.Actions = 1;
		}

		private static string[] choices = new string[] { "Cards", "Gain", "Remodel" };
		private static string[] choiceDescriptions = new string[] { "Each player gets +1(+3) Cards", "Each player gains a Silver(Gold)", "Each player may trash a card from his hand and gain a card costing exactly 1(2) more" };
		public override void Play(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Governor, "Choose one; you get the version in parenthesis", choices, choiceDescriptions);
				
			switch (choice)
			{
				case 0:
					gameModel.CurrentPlayer.Draw(3);
					foreach (Player player in gameModel.Players)
					{
						if (player != gameModel.CurrentPlayer)
						{
							player.Draw(1);
						}
					}
					break;
				case 1:
					gameModel.CurrentPlayer.GainCard(typeof(Gold));
					foreach (Player player in gameModel.Players)
					{
						if (player != gameModel.CurrentPlayer)
						{
							player.GainCard(typeof(Silver));
						}
					}
					break;
				case 2:
					CardModel card= gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashForGovernor, "You may trash a card from hand and gain one costing exactly 2 more", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
					if (card != null)
					{
						gameModel.CurrentPlayer.Trash(card);
						Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing exactly 2 more", gameModel.SupplyPiles.Where(p => p.Count > 0 && p.GetCost() == gameModel.GetCost(card) + 2 && p.CostsPotion == card.CostsPotion));
						if (pile != null)
						{
							gameModel.CurrentPlayer.GainCard(pile);
						}
					}

					foreach (Player player in gameModel.Players)
					{
						if (player != gameModel.CurrentPlayer)
						{
							CardModel card2 = player.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashForUpgrade, "You may trash a card from hand and gain one costing exactly 1 more", Chooser.ChoiceSource.FromHand, player.Hand);
							if (card2 != null)
							{
								player.Trash(card2);
								Pile pile = player.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing exactly 1 more", gameModel.SupplyPiles.Where(p => p.Count > 0 && p.GetCost() == gameModel.GetCost(card2) + 1 && p.CostsPotion == card2.CostsPotion));
								if (pile != null)
								{
									player.GainCard(pile);
								}
							}
						}
					}
					break;
			}
		}

		public override CardModel Clone()
		{
			return new Governor();
		}
	}
}