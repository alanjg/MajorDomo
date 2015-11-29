using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Swindler : IntrigueCardModel
	{
		public Swindler()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Swindler";
			this.Cost = 3;
			this.Coins = 2;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			Player currentPlayer = gameModel.CurrentPlayer;

			foreach(Player player in attackedPlayers)
			{
				CardModel trashedCard = player.DrawCard();
				if (trashedCard != null)
				{
					int cost = gameModel.GetCost(trashedCard);
					player.Trash(trashedCard);
					Pile pile = currentPlayer.Chooser.ChooseOnePile(CardChoiceType.ForceGain, "Pick a card for " + player.Name + " to gain", gameModel.SupplyPiles.Where(p => p.Count > 0 && gameModel.GetCost(p) == cost && p.CostsPotion == trashedCard.CostsPotion));
					if (pile != null)
					{
						player.GainCard(pile);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Swindler();
		}
	}
}
