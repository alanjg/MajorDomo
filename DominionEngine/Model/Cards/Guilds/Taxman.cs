using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Taxman : GuildsCardModel
	{
		public Taxman()
		{
			this.Name = "Taxman";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 4;
		}
		public override CardModel Clone()
		{
			Taxman clone = (Taxman)base.Clone();
			clone.choice = this.choice != null ? this.choice.Clone() : null;
			return clone;
		}

		private CardModel choice = null;
		public override void Play(GameModel gameModel)
		{
			this.choice = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashForTaxman, "You may trash a Treasure from your hand.", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Treasure)));
			if (this.choice != null)
			{
				gameModel.CurrentPlayer.Trash(this.choice);
			}
		}

		public override void PlayAttack(GameModel gameModel, System.Collections.Generic.IEnumerable<Player> attackedPlayers)
		{
			if (this.choice != null)
			{
				foreach (Player player in attackedPlayers)
				{
					if (player.Hand.Count >= 5)
					{
						CardModel match = player.Hand.FirstOrDefault(c => c.Name == this.choice.Name);
						if (match != null)
						{
							player.DiscardCard(match);
						}
						else
						{
							player.RevealHand();
						}
					}
				}
			}
		}

		public override void PlayPostAttack(GameModel gameModel)
		{
			if (this.choice != null)
			{
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.GainOnTopOfDeck, "Gain a treasure costing up to $" + (gameModel.GetCost(this.choice) + 3).ToString() + (this.choice.CostsPotion ? "P" : ""), gameModel.SupplyPiles.Where(p => p.Count > 0 && p.Card.Is(CardType.Treasure) && p.GetCost() <= gameModel.GetCost(this.choice) + 3 && (!p.CostsPotion || this.choice.CostsPotion)));
				if (pile != null)
				{
					gameModel.CurrentPlayer.GainCard(pile, GainLocation.TopOfDeck);
				}
			}
		}
	}
}
