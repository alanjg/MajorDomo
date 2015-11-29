using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Mountebank : ProsperityCardModel
	{
		public Mountebank()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Mountebank";
			this.Cost = 5;
			this.Coins = 2;
		}

		private static string[] choices = new string[] { "Discard", "Gain" };
		private static string[] choiceDescriptions = new string[] { "Discard Curse", "Gain a Curse and a Copper" };
		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				CardModel curse = player.Hand.FirstOrDefault(card => card is Curse);
				if (curse != null)
				{
					int choice = player.Chooser.ChooseOneEffect(EffectChoiceType.DiscardForMountebank, "You may discard a Curse", choices, choiceDescriptions);
					if (choice == 0)
					{
						player.DiscardCard(curse);
					}
					else
					{
						player.GainCard(typeof(Curse));
						player.GainCard(typeof(Copper));
					}
				}
				else
				{
					player.GainCard(typeof(Curse));
					player.GainCard(typeof(Copper));
				}
			}
		}

		public override CardModel Clone()
		{
			return new Mountebank();
		}
	}
}