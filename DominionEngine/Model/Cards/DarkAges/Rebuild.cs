using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Rebuild : DarkAgesCardModel
	{
		public Rebuild()
		{
			this.Name = "Rebuild";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.NameACardForRebuild, "Name a card", Chooser.ChoiceSource.None, gameModel.AllCardsInGame.Where(c => c.Is(CardType.Victory)).Union(new CardModel[] { new Copper() }));
			CardModel found = null;
			List<CardModel> setAside = new List<CardModel>();
			do
			{
				CardModel card = gameModel.CurrentPlayer.DrawCard();
				if (card != null && card.Is(CardType.Victory) && card.Name != choice.Name)
				{
					found = card;
				}
				else
				{
					if (card == null)
					{
						break;
					}
					setAside.Add(card);
				}
			} while (found == null);
					
			foreach (CardModel card in setAside)
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}

			if (found != null)
			{
				gameModel.CurrentPlayer.Trash(found);
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a Victory card costing up to $" + (gameModel.GetCost(found) + 3).ToString() + (found.CostsPotion ? "P" : ""), gameModel.SupplyPiles.Where(p => p.Count > 0 && p.TopCard.Is(CardType.Victory) && p.GetCost() <= gameModel.GetCost(found) + 3 && (!p.CostsPotion || found.CostsPotion)));
				if (pile != null)
				{
					gameModel.CurrentPlayer.GainCard(pile);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Rebuild();
		}
	}
}