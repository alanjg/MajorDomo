using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Mandarin : HinterlandsCardModel
	{
		public Mandarin()
		{
			this.Name = "Mandarin";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Coins = 3;
		}

		public override void Play(GameModel gameModel)
		{
			if (gameModel.CurrentPlayer.Hand.Count > 0)
			{
				CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.PutOnDeckFromHand, this, "Put a card back on your deck", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				gameModel.CurrentPlayer.RemoveFromHand(choice);
				gameModel.CurrentPlayer.Deck.PlaceOnTop(choice);
			}
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			IEnumerable<CardModel> treasures = player.Played.Where(card => card.Is(CardType.Treasure));
			IEnumerable<CardModel> order = player.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Choose the order to place treasures on top of your deck(first on top)", treasures);
			foreach (CardModel treasure in order.Reverse().ToList())
			{
				player.RemoveFromPlayed(treasure);
				player.Deck.PlaceOnTop(treasure);
			}
		}

		public override CardModel Clone()
		{
			return new Mandarin();
		}
	}
}
