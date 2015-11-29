using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Duchess : HinterlandsCardModel
	{
		public Duchess()
		{
			this.Name = "Duchess";
			this.Type = CardType.Action;
			this.Cost = 2;
		}

		private static string[] choices = new string[] { "Keep", "Discard" };
		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			currentPlayer.AddActionCoin(2);
			foreach (Player player in gameModel.Players)
			{
				CardModel top = player.DrawCard();
				if (top != null)
				{
					gameModel.TextLog.WriteLine(player.Name + " revealed " + top.Name);
					int choice = player.Chooser.ChooseOneEffect(EffectChoiceType.DiscardOrPutOnDeck, top, "Keep or discard", choices, choices);
					if (choice == 0)
					{
						player.Deck.PlaceOnTop(top);
					}
					else
					{
						player.DiscardCard(top);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Duchess();
		}
	}
}
