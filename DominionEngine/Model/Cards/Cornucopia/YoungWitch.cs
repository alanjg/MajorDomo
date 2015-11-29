using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class YoungWitch : CornucopiaCardModel
	{
		public YoungWitch()
		{
			this.Name = "Young Witch";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 4;
			this.Cards = 2;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.DiscardCards(2);
		}

		private static string[] choices = new string[] { "Yes", "No" };

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				CardModel bane = player.Hand.FirstOrDefault(card => card.GetType() == gameModel.Bane.GetType());
				if (bane != null)
				{
					int choice = player.Chooser.ChooseOneEffect(EffectChoiceType.RevealBane, "Do you want to reveal a bane?", choices, choices);
					if (choice == 0)
					{
						player.RevealCardFromHand(bane);
					}
					else
					{
						player.GainCard(typeof(Curse));
					}
				}
				else
				{
					player.GainCard(typeof(Curse));
				}
			}
		}

		public override CardModel Clone()
		{
			return new YoungWitch();
		}
	}
}
