using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Torturer : IntrigueCardModel
	{
		public Torturer()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Torturer";
			this.Cost = 5;
			this.Cards = 3;
		}

		private static string[] choices = new string[] { "Discard", "Curse" };
		private static string[] choiceDescriptions = new string[] { "Discard 2 cards", "Gain a curse in hand" };

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				int choice = player.Chooser.ChooseOneEffect(EffectChoiceType.Torturer, "Torturer", choices, choiceDescriptions);
				if (choice == 0)
				{
					IEnumerable<CardModel> c = player.Chooser.ChooseSeveralCards(CardChoiceType.Discard, "Discard 2 cards", Chooser.ChoiceSource.FromHand, 2, 2, player.Hand.ToList());
					foreach (CardModel discard in c)
					{
						player.DiscardCard(discard);
					}
				}
				else
				{
					player.GainCard(typeof(Curse), GainLocation.InHand);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Torturer();
		}
	}
}
