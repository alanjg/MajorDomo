using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Saboteur : IntrigueCardModel
	{
		public Saboteur()
		{
			this.Name = "Saboteur";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			Player currentPlayer = gameModel.CurrentPlayer;

			foreach (Player player in attackedPlayers)
			{
				List<CardModel> setAside = new List<CardModel>();
				CardModel card = player.DrawCard();
				while (card != null)
				{
					gameModel.TextLog.WriteLine(player.Name + " reveals " + card.Name);
					if (gameModel.GetCost(card) >= 3)
					{
						break;
					}
					else
					{
						setAside.Add(card);
					}
					card = player.DrawCard();
				}

				if (card != null)
				{
					player.Trash(card);
					Pile choice = player.Chooser.ChooseZeroOrOnePile(CardChoiceType.Gain, "You may gain a card costing up to $" + (gameModel.GetCost(card) - 2).ToString(), 
						gameModel.SupplyPiles.Where(p => gameModel.GetCost(p) <= gameModel.GetCost(card) - 2 && (card.CostsPotion || !p.CostsPotion) && p.Count > 0));

					if (choice != null)
					{
						player.GainCard(choice);
					}
				}

				foreach (CardModel temp in setAside)
				{
					player.DiscardCard(temp);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Saboteur();
		}
	}
}
