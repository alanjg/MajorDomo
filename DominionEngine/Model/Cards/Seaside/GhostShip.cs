using System;
using System.Linq;
using Dominion.Model.Chooser;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class GhostShip : SeasideCardModel
	{
		public GhostShip()
		{
			this.Type = CardType.Action | CardType.Attack; ;
			this.Name = "Ghost Ship";
			this.Cost = 5;
			this.Cards = 2;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				Player cachedPlayer = player;

				int numToDiscard = player.Hand.Count - 3;
				if (numToDiscard > 0)
				{
					IEnumerable<CardModel> discards = player.Chooser.ChooseSeveralCards(CardChoiceType.PutOnDeckFromHand, this, "Return " + numToDiscard + " cards to your deck(first on top)", ChoiceSource.FromHand, numToDiscard, numToDiscard, player.Hand);
					foreach (CardModel discard in discards.Reverse().ToArray())
					{
						cachedPlayer.RemoveFromHand(discard);
						cachedPlayer.Deck.PlaceOnTop(discard);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new GhostShip();
		}
	}
}