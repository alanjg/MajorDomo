using System;
using System.Collections.Generic;
using Dominion.Model.Chooser;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class Vault : ProsperityCardModel
	{
		private static string[] vaultEffects = new string[] { "Discard", "Nothing" };
		private static string[] vaultEffectDescriptions = new string[] { "Discard 2 cards", "Do nothing" };
		public Vault()
		{
			this.Type = CardType.Action;
			this.Name = "Vault";
			this.Cost = 5;
			this.Cards = 2;
		}
		
		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> choices = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.DiscardForCoins, "Choose cards to discard to Vault.", Chooser.ChoiceSource.FromHand, 0, gameModel.CurrentPlayer.Hand.Count, gameModel.CurrentPlayer.Hand);
			int coin = 0;
			foreach (CardModel discard in choices.ToList())
			{
				gameModel.CurrentPlayer.DiscardCard(discard);
				coin++;
			}
			gameModel.CurrentPlayer.AddActionCoin(coin);
			foreach (Player otherPlayer in gameModel.Players)
			{
				if (otherPlayer != gameModel.CurrentPlayer)
				{
					Player cachedPlayer = otherPlayer;
					int choice = cachedPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Vault, "You may discard 2 cards to draw one card.", vaultEffects, vaultEffectDescriptions);
					if (choice == 0)
					{
						int cardsToChoose = Math.Min(cachedPlayer.Hand.Count, 2);
						choices = cachedPlayer.Chooser.ChooseSeveralCards(CardChoiceType.Discard, "Choose 2 cards to discard", Chooser.ChoiceSource.FromHand, cardsToChoose, cardsToChoose, cachedPlayer.Hand);
						int discarded = 0;
						foreach (CardModel card in choices.ToList())
						{
							cachedPlayer.DiscardCard(card);
							discarded++;
						}
						if (discarded == 2)
						{
							cachedPlayer.Draw();
						}
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Vault();
		}
	}
}