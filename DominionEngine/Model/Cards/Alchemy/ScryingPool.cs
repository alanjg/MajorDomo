using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class ScryingPool : AlchemyCardModel
	{
		public ScryingPool()
		{
			this.Name = "Scrying Pool";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 2;
			this.CostsPotion = true;
			this.Actions = 1;
		}

		private static string[] choices = new string[] { "Keep", "Discard" };
		
		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			CardModel topCard = currentPlayer.DrawCard();
			if (topCard != null)
			{
				int choice = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardOrPutOnDeckToDraw, topCard, "You reveal " + topCard.Name, choices, choices);
				if (choice == 0)
				{
					currentPlayer.Deck.PlaceOnTop(topCard);
				}
				else
				{
					currentPlayer.DiscardCard(topCard);
				}
			}
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			foreach (Player player in attackedPlayers)
			{
				CardModel topCard = player.DrawCard();
				if (topCard != null)
				{
					int choice = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ForceDiscardOrPutOnDeck, topCard, player.Name + " reveals " + topCard.Name, choices, choices);
					if (choice == 0)
					{
						player.Deck.PlaceOnTop(topCard);
					}
					else
					{
						player.DiscardCard(topCard);
					}
				}
			}
		}

		public override void PlayPostAttack(GameModel gameModel)
		{
			List<CardModel> cards = new List<CardModel>();
			CardModel revealedCard = null;
			do
			{
				revealedCard = gameModel.CurrentPlayer.DrawCard();
				if (revealedCard != null)
				{
					cards.Add(revealedCard);
				}
			} while (revealedCard != null && revealedCard.Is(CardType.Action));

			foreach (CardModel card in cards)
			{
				gameModel.CurrentPlayer.PutInHand(card);
			}
		}

		public override CardModel Clone()
		{
			return new ScryingPool();
		}
	}
}
