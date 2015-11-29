using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Rogue : DarkAgesCardModel
	{
		public Rogue()
		{
			this.Name = "Rogue";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
			this.Coins = 2;
		}

		public override CardModel Clone()
		{
			Rogue clone = (Rogue)base.Clone();
			clone.foundTrash = this.foundTrash;
			return clone;
		}

		private bool foundTrash = false;
		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> trash = gameModel.Trash.Where(c => gameModel.GetCost(c) >= 3 && gameModel.GetCost(c) <= 6 && !c.CostsPotion);
			if (trash.Any())
			{
				this.foundTrash = true;
				CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.Gain, "Gain a card costing between $3 and $6 from the trash.", Chooser.ChoiceSource.FromTrash, trash);
				gameModel.CurrentPlayer.GainCard(card, null);
				gameModel.Trash.Remove(card);
			}
			else
			{
				this.foundTrash = false;
			}
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			if (!this.foundTrash)
			{
				foreach (Player player in attackedPlayers)
				{
					IEnumerable<CardModel> cards = player.DrawCards(2);
					IEnumerable<CardModel> eligible = cards.Where(c => gameModel.GetCost(c) >= 3 && gameModel.GetCost(c) <= 6 && !c.CostsPotion);
					CardModel choice = player.Chooser.ChooseOneCard(CardChoiceType.Trash, "Choose a card to trash", Chooser.ChoiceSource.None, eligible);
					if (choice != null)
					{
						player.Trash(choice);
					}
					foreach (CardModel card in cards)
					{
						if (card != choice)
						{
							player.DiscardCard(card);
						}
					}
				}
			}
		}
	}
}