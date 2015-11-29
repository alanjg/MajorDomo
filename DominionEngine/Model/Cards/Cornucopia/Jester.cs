using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Jester : CornucopiaCardModel
	{
		public Jester()
		{
			this.Name = "Jester";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
			this.Coins = 2;
		}

		private static string[] choices = new string[] { "You", "Them" };
		
		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				CardModel topCard = player.DrawCard();
				if (topCard != null)
				{
					if (topCard.Is(CardType.Victory))
					{
						player.GainCard(typeof(Curse));
					}
					else
					{
						gameModel.TextLog.WriteLine(player.Name + " reveals a " + topCard.Name + ".");
						int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.GainForJester, topCard, "Who gains a copy?", choices, choices);
						if (choice == 0)
						{
							gameModel.CurrentPlayer.GainCard(topCard.GetType());
						}
						else
						{
							player.GainCard(topCard.GetType());
						}
					}
					player.DiscardCard(topCard);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Jester();
		}
	}
}
