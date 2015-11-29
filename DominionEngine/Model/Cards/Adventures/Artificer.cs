using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Artificer : AdventuresCardModel
	{
		public Artificer()
		{
			this.Name = "Artificer";
			this.Type = CardType.Action;
			this.Cost = 5;
			
			this.Actions = 1;
			this.Cards = 1;
			this.Coins = 1;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> cards = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(Chooser.CardChoiceType.DiscardForArtificer, "Discard any number of cards", Chooser.ChoiceSource.FromHand, 0, gameModel.CurrentPlayer.Hand.Count, gameModel.CurrentPlayer.Hand).ToArray();
			foreach(CardModel card in cards)
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}
			int cost = cards.Count();
			Pile pile = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOnePile(Chooser.CardChoiceType.GainOnTopOfDeck, "You may gain a card costing $" + cost, gameModel.SupplyPiles.Where(p => p.Cost == cost && p.Count > 0 && !p.CostsPotion));
			if (pile != null)
			{
				gameModel.CurrentPlayer.GainCard(pile, GainLocation.TopOfDeck);
			}
		}

		public override CardModel Clone()
		{
			return new Artificer();
		}
	}
}
