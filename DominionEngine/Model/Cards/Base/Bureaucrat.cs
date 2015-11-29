using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Bureaucrat : BaseCardModel
	{
		public Bureaucrat()
		{
			this.Name = "Bureaucrat";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Silver), GainLocation.TopOfDeck);
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				IEnumerable<CardModel> victoryCards = player.Hand.Where(v => v.Is(CardType.Victory));

				if (victoryCards.Any())
				{
					CardModel victoryCard = player.Chooser.ChooseOneCard(CardChoiceType.PutOnDeckFromHand, this, "Put a victory card back on the deck", ChoiceSource.FromHand, victoryCards);
					player.RemoveFromHand(victoryCard);
					gameModel.TextLog.WriteLine(player.Name + " reveals " + victoryCard.Name + " and places it on the top of his deck.");
					player.Deck.PlaceOnTop(victoryCard);
				}
				else
				{
					player.RevealHand();
				}				
			}
		}

		public override CardModel Clone()
		{
			return new Bureaucrat();
		}
	}
}
