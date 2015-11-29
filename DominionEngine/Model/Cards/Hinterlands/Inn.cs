using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Inn : HinterlandsCardModel
	{
		public Inn()
		{
			this.Name = "Inn";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 2;
			this.Cards = 2;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.DiscardCards(2);
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			IEnumerable<CardModel> discardedActions = player.Discard.Where(card => card.Is(CardType.Action));
			IEnumerable<CardModel> choices = player.Chooser.ChooseSeveralCards(CardChoiceType.Inn, "Choose actions to shuffle into your deck", ChoiceSource.None, 0, discardedActions.Count(), discardedActions);
			foreach(CardModel choice in choices.ToArray())
			{
				player.RemoveFromDiscard(choice);
				player.Deck.PlaceOnTop(choice);
			}

			player.Deck.Shuffle();
		}

		public override CardModel Clone()
		{
			return new Inn();
		}
	}
}
