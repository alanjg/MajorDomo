using System;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class Tribute : IntrigueCardModel
	{
		public Tribute()
		{
			this.Type = CardType.Action;
			this.Name = "Tribute";
			this.Cost = 5;
		}
		
		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			Player nextPlayer = gameModel.Players[(gameModel.CurrentPlayerIndex + 1) % gameModel.Players.Count];

			int bonusActions = 0;
			int bonusCoins = 0;
			int bonusCards = 0;

			List<CardModel> cards = nextPlayer.DrawCards(2);
			nextPlayer.RevealCards(cards);
			foreach (CardModel card in cards)
			{
				nextPlayer.DiscardCard(card);
			}
			if(cards.Count == 2 && cards[0].Name == cards[1].Name)
			{
				cards.RemoveAt(1);
			}
			foreach(CardModel card in cards)
			{
				if (card.Is(CardType.Action))
				{
					bonusActions += 2;
				}
				if (card.Is(CardType.Treasure))
				{
					bonusCoins += 2;
				}
				if (card.Is(CardType.Victory))
				{
					bonusCards += 2;
				}
			}

			string log = currentPlayer.Name + " gets ";
			bool any = false;
			if (bonusActions > 0)
			{
				log += "+" + bonusActions + " actions";
				any = true;
			}
			if (bonusCoins > 0)
			{
				if (any)
				{
					log += ", ";
				}
				log += "+" + bonusCoins + " coins";
				any = true;
			}
			if (bonusCards > 0)
			{
				if (any)
				{
					log += ", ";
				}
				log += "+" + bonusCards + " cards";
			}
			log += ".";
			gameModel.TextLog.WriteLine(log);
			currentPlayer.GainActions(bonusActions);
			currentPlayer.AddActionCoin(bonusCoins);
			currentPlayer.Draw(bonusCards);				
		}

		public override CardModel Clone()
		{
			return new Tribute();
		}
	}
}
