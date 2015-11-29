using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Graverobber : DarkAgesCardModel
	{
		public Graverobber()
		{
			this.Name = "Graverobber";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		private static string[] choices = new string[] { "Hand", "Trash" };
		private static string[] choiceText = new string[] { "Trash an action in hand", "Gain a card from the trash" };

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> cardsInTrash = gameModel.Trash.Where(c => gameModel.GetCost(c) >= 3 && gameModel.GetCost(c) <= 6 && !c.CostsPotion);
			IEnumerable<CardModel> cardsInHand = gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Action));

			bool gainFromTrash = false;
			if (cardsInHand.Any() && cardsInTrash.Any())
			{
				int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Graverobber, "Choose one", choices, choiceText);
				gainFromTrash = choice == 1;
			}
			else if(cardsInHand.Any() && !cardsInTrash.Any())
			{
				gainFromTrash = false;
			}
			else if (!cardsInHand.Any() && cardsInTrash.Any())
			{
				gainFromTrash = true;
			}
			else
			{
				return;
			}
			if (gainFromTrash)
			{
				CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.GainOnTopOfDeck, "Gain a card from the trash costing between $3 and $6", Chooser.ChoiceSource.FromTrash, cardsInTrash);
				gameModel.Trash.Remove(choice);
				gameModel.CurrentPlayer.GainCard(choice, null, GainLocation.TopOfDeck);
			}
			else
			{
				CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForGraverobber, "Trash an action from your hand", Chooser.ChoiceSource.FromHand, cardsInHand.Where(c => c.Is(CardType.Action)));
				gameModel.CurrentPlayer.Trash(choice);
				IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(p => p.Count > 0 && p.GetCost() <= gameModel.GetCost(choice) + 3 && (!p.CostsPotion || choice.CostsPotion));
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to $" + (gameModel.GetCost(choice) + 3).ToString(), piles);
				if (pile != null)
				{
					gameModel.CurrentPlayer.GainCard(pile);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Graverobber();
		}
	}
}