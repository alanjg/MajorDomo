using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class BlackMarket : PromoCardModel
	{
		public BlackMarket()
		{
			this.Type = CardType.Action;
			this.Name = "Black Market";
			this.Cost = 3;
			this.Coins = 2;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> blackMarket = gameModel.BlackMarket.Take(3).ToArray();
			
			gameModel.TextLog.Write(gameModel.CurrentPlayer.Name + " reveals ");
			foreach (CardModel card in blackMarket)
			{
				gameModel.BlackMarket.Remove(card);
			}
			gameModel.TextLog.WriteSortedCards(blackMarket);
			gameModel.TextLog.WriteLine();
			
			bool go = true;
			while (go)
			{
				IEnumerable<CardModel> affordable = blackMarket.Where(c => gameModel.GetCost(c) <= gameModel.CurrentPlayer.Coin && (!c.CostsPotion || gameModel.CurrentPlayer.Potions > 0) && c.CanBuy(gameModel.CurrentPlayer));
				CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.BlackMarket, "You may play a treasure from hand or purchase a card from the Black Market", Chooser.ChoiceSource.None | Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Treasure)).Union(affordable));
				if (choice != null)
				{
					if (affordable.Contains(choice))
					{
						gameModel.CurrentPlayer.BuyBlackMarketCard(choice);
						blackMarket = blackMarket.Where(c => c != choice);
						go = false;
					}
					else
					{
						gameModel.CurrentPlayer.PlayTreasure(choice);
					}
				}
				else
				{
					go = false;
				}
			}
			foreach (CardModel card in gameModel.CurrentPlayer.Chooser.ChooseOrder(CardOrderType.OrderInBlackMarket, "Put the remaining cards back in any order(first on top)", blackMarket))
			{
				gameModel.BlackMarket.Add(card);
			}
		}

		public override CardModel Clone()
		{
			return new BlackMarket();
		}
	}
}