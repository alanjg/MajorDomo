using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Library : BaseCardModel
	{
		public Library()
		{
			this.Name = "Library";
			this.Cost = 5;
			this.Type = CardType.Action;
		}

		private static string[] choices = new string[] { "Keep", "SetAside" };
		private static string[] choiceDescriptions = new string[] { "Keep", "Set aside" };
		public override void Play(GameModel gameModel)
		{
			List<CardModel> setAside = new List<CardModel>();

			Player currentPlayer = gameModel.CurrentPlayer;
			while (currentPlayer.Hand.Count < 7)
			{
				CardModel cardToAdd = currentPlayer.DrawCard();
				if (cardToAdd != null)
				{
					if (cardToAdd.Is(CardType.Action))
					{
						int shouldDraw = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.SetAsideForLibrary, cardToAdd, "You draw " + cardToAdd.Name, choices, choiceDescriptions);
						if (shouldDraw == 0)
						{
							currentPlayer.PutInHand(cardToAdd);
						}
						else
						{
							gameModel.TextLog.WriteLine(currentPlayer.Name + " sets a " + cardToAdd.Name + " aside.");
							setAside.Add(cardToAdd);
						}
					}
					else
					{
						currentPlayer.PutInHand(cardToAdd);
					}
				}
				else
				{
					break;
				}				
			}

			foreach (CardModel card in setAside)
			{
				currentPlayer.DiscardCard(card);
			}
		}

		public override CardModel Clone()
		{
			return new Library();
		}
	}
}
