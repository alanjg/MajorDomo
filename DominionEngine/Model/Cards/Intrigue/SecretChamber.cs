using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class SecretChamber : IntrigueCardModel
	{
		public SecretChamber()
		{
			this.Type = CardType.Action | CardType.Reaction;
			this.Name = "Secret Chamber";
			this.Cost = 2;
			this.ReactionTrigger = Dominion.ReactionTrigger.AttackPlayed;
		}

		public override bool ReactToAttack(GameModel gameModel, Player targetPlayer)
		{
			Player currentPlayer = gameModel.CurrentPlayer;

			targetPlayer.Draw();
			targetPlayer.Draw();

			IEnumerable<CardModel> choices = targetPlayer.Chooser.ChooseSeveralCards(CardChoiceType.PutOnDeckFromHand, this, "Place 2 cards on your deck(first on top)", Chooser.ChoiceSource.FromHand, 2, 2, targetPlayer.Hand);
			IList<CardModel> discards = choices.ToList();
			for (int i = discards.Count - 1; i >= 0; i--)
			{
				CardModel discard = discards[i];
				targetPlayer.RemoveFromHand(discard);
				targetPlayer.Deck.PlaceOnTop(discard);
			}
			return false;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			IEnumerable<CardModel> choices = currentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.DiscardForCoins, "Discard cards for +1 coin each", Chooser.ChoiceSource.FromHand, 0, currentPlayer.Hand.Count, currentPlayer.Hand);
			IList<CardModel> discards = choices.ToList();
			for (int i = discards.Count - 1; i >= 0; i--)
			{
				CardModel discard = discards[i];
				currentPlayer.DiscardCard(discard);
			}
			currentPlayer.AddActionCoin(discards.Count);
			gameModel.TextLog.WriteLine(currentPlayer.Name + " gains " + discards.Count + " coin.");
		}

		public override CardModel Clone()
		{
			return new SecretChamber();
		}
	}
}
