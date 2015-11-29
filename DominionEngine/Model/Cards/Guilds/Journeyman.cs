using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Journeyman : GuildsCardModel
	{
		public Journeyman()
		{
			this.Name = "Journeyman";
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel namedCard = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.NameACardForJourneyman, "Name a card", ChoiceSource.None, gameModel.AllCardsInGame);
			List<CardModel> match = new List<CardModel>();
			List<CardModel> nonMatch = new List<CardModel>();
			CardModel card = gameModel.CurrentPlayer.DrawCard();
			while (card != null)
			{
				if (card.Name == namedCard.Name)
				{
					match.Add(card);
				}
				else
				{
					nonMatch.Add(card);
					if(nonMatch.Count == 3)
					{
						break;
					}
				}
				card = gameModel.CurrentPlayer.DrawCard();
			}
			foreach (CardModel nonMatched in nonMatch)
			{
				gameModel.CurrentPlayer.PutInHand(nonMatched);
			}
			foreach (CardModel matched in match)
			{
				gameModel.CurrentPlayer.DiscardCard(matched);
			}
		}

		public override CardModel Clone()
		{
			return new Journeyman();
		}
	}
}
