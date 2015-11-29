using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Thief : BaseCardModel
	{
		public Thief()
		{
			this.Name = "Thief";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 4;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			List<CardModel> loot = new List<CardModel>();

			foreach (Player player in attackedPlayers)
			{
				List<CardModel> cards = player.DrawCards(2);

				IEnumerable<CardModel> treasures = (from c in cards where c.Is(CardType.Treasure) select c);
				if (treasures.Any())
				{
					CardModel treasure = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForThief, "Trash a treasure", ChoiceSource.None, treasures);
					if (treasure != null)
					{
						player.Trash(treasure);
						loot.Add(treasure);
						cards.Remove(treasure);
					}
				}
				foreach (CardModel card in cards)
				{
					player.DiscardCard(card);
				}
			}

			IEnumerable<CardModel> chosenLoot = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.Gain, "Gain any trashed treasures", ChoiceSource.None, 0, loot.Count, loot);
			foreach (CardModel gainedCard in chosenLoot)
			{
				gameModel.Trash.Remove(gainedCard);
				gameModel.CurrentPlayer.GainCard(gainedCard, null);
			}
		}

		public override CardModel Clone()
		{
			return new Thief();
		}
	}
}
