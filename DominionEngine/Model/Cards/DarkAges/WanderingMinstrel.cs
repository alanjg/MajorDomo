using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;


namespace Dominion.Model.Actions
{
	public class WanderingMinstrel : DarkAgesCardModel
	{
		public WanderingMinstrel()
		{
			this.Name = "Wandering Minstrel";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Cards = 1;
			this.Actions = 2;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> cards = gameModel.CurrentPlayer.DrawCards(3);
			IEnumerable<CardModel> actions = cards.Where(c => c.Is(CardType.Action));
			IEnumerable<CardModel> order = gameModel.CurrentPlayer.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put the actions back in any order(first on top)", actions);
			foreach (CardModel card in order.Reverse())
			{
				gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
			}
			foreach (CardModel card in cards.Where(c => !c.Is(CardType.Action)))
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}
		}

		public override CardModel Clone()
		{
			return new WanderingMinstrel();
		}
	}
}