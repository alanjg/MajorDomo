using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Herbalist : AlchemyCardModel
	{
		public Herbalist()
		{
			this.Name = "Herbalist";
			this.Type = CardType.Action;
			this.Cost = 2;
			this.Coins = 1;
			this.Buys = 1;
		}

		public override bool HasCleanupEffect(GameModel gameModel)
		{
			return gameModel.CurrentPlayer.Played.Any(card => card.Is(CardType.Treasure));
		}

		public override void OnCleanup(GameModel gameModel)
		{
			CardModel chosen = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.PutOnDeckFromPlayed, "You may return a treasure in play to the top of your deck", ChoiceSource.InPlay, gameModel.CurrentPlayer.Played.Where(card => card.Is(CardType.Treasure)));
			if (chosen != null)
			{
				gameModel.CurrentPlayer.RemoveFromPlayed(chosen);
				gameModel.CurrentPlayer.Deck.PlaceOnTop(chosen);
			}
		}

		public override CardModel Clone()
		{
			return new Herbalist();
		}
	}
}
