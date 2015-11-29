using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Oracle : HinterlandsCardModel
	{
		public Oracle()
		{
			this.Name = "Oracle";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 3;
		}

		private static string[] choices = new string[] { "Keep", "Discard" };
		private void DoAttack(Player attacker, Player attacked)
		{
			List<CardModel> revealed = attacked.DrawCards(2);
			if (revealed.Count > 0)
			{
				string cards = Log.FormatSortedCards(revealed);

				string target = attacked == attacker ? "You" : attacked.Name;
				int choice = attacker.Chooser.ChooseOneEffect(attacker == attacked ? EffectChoiceType.DiscardOrPutOnDeckToDraw: EffectChoiceType.ForceDiscardOrPutOnDeck, revealed, target + " revealed " + cards, choices, choices);
				if (choice == 0)
				{
					IEnumerable<CardModel> order = attacked.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put " + cards + " back in any order", revealed);
					foreach (CardModel c in order)
					{
						attacked.Deck.PlaceOnTop(c);
					}
				}
				else
				{
					foreach (CardModel c in revealed)
					{
						attacked.DiscardCard(c);
					}
				}
			}
		}

		public override void Play(GameModel gameModel)
		{
			this.DoAttack(gameModel.CurrentPlayer, gameModel.CurrentPlayer);
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player attacked in attackedPlayers)
			{
				this.DoAttack(gameModel.CurrentPlayer, attacked);
			}
		}

		public override void PlayPostAttack(GameModel gameModel)
		{
			gameModel.CurrentPlayer.Draw(2);
		}

		public override CardModel Clone()
		{
			return new Oracle();
		}
	}
}
