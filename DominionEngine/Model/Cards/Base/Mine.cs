using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Mine : BaseCardModel
	{
		public Mine()
		{
			this.Name = "Mine";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> treasures = gameModel.CurrentPlayer.Hand.Where(card => card.Is(CardType.Treasure));
			Player currentPlayer = gameModel.CurrentPlayer;
			if (treasures.Any())
			{
				CardModel chosenCard = currentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForMine, "Trash a treasure from your hand", ChoiceSource.FromHand, treasures);
				currentPlayer.Trash(chosenCard);
				Pile newCardPile = currentPlayer.Chooser.ChooseOnePile(CardChoiceType.GainInHand, "Gain a treasure costing up to $" + (gameModel.GetCost(chosenCard) + 3).ToString(),
					gameModel.SupplyPiles.Where(pile => pile.Card.Is(CardType.Treasure) && gameModel.GetCost(pile) <= gameModel.GetCost(chosenCard) + 3 &&
						(chosenCard.CostsPotion || !pile.CostsPotion)));
				if (newCardPile != null)
				{
					currentPlayer.GainCard(newCardPile, GainLocation.InHand);
				}				
			}
		}

		public override CardModel Clone()
		{
			return new Mine();
		}
	}
}
