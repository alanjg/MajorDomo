using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Catacombs : DarkAgesCardModel
	{
		public Catacombs()
		{
			this.Name = "Catacombs";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		private static string[] choices = new string[] { "Keep", "Discard" };
		public override void Play(GameModel gameModel)
		{
			List<CardModel> cards = gameModel.CurrentPlayer.DrawCards(3);
			
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardOrPutOnDeckToDraw, cards, "You drew " + Log.FormatSortedCards(cards), choices, choices);
			if (choice == 0)
			{
				foreach (CardModel card in cards)
				{
					gameModel.CurrentPlayer.PutInHand(card);
				}
			}
			else
			{
				foreach (CardModel card in cards)
				{
					gameModel.CurrentPlayer.DiscardCard(card);
				}
				gameModel.CurrentPlayer.Draw(3);
			}
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			IEnumerable<Pile> piles = gameModel.SupplyPiles.Where(p => (p.GetCost() < gameModel.GetCost(this) && (!p.CostsPotion || this.CostsPotion)) || p.GetCost() == gameModel.GetCost(this) && !p.CostsPotion && this.CostsPotion);
			if(piles.Any())
			{
				Pile choice = owner.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing less than $" + gameModel.GetCost(this), piles);
				owner.GainCard(choice);
			}
		}

		public override CardModel Clone()
		{
			return new Catacombs();
		}
	}
}