using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Apothecary : AlchemyCardModel
	{
		public Apothecary()
		{
			this.Name = "Apothecary";
			this.Cost = 2;
			this.CostsPotion = true;
			this.Actions = 1;
			this.Cards = 1;
			this.Type = CardType.Action;
		}

		public override void Play(GameModel gameModel)
		{
			Player player = gameModel.CurrentPlayer;
			List<CardModel> cards = player.DrawCards(4);

			player.RevealCards(cards);

			IEnumerable<CardModel> potionsAndCoppers = cards.Where(c => c is Potion || c is Copper).ToList();

			foreach (CardModel card in potionsAndCoppers)
			{
				player.PutInHand(card);
				cards.Remove(card);
			}

			if (cards.Any())
			{
				IEnumerable<CardModel> order = player.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put the rest back in any order(first on top)", cards);
				foreach (CardModel card in order.Reverse().ToList())
				{
					player.Deck.PlaceOnTop(card);
				}
			}			
		}

		public override CardModel Clone()
		{
			return new Apothecary();
		}
	}
}
