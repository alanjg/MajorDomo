using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class PearlDiver : SeasideCardModel
	{
		public PearlDiver()
		{
			this.Type = CardType.Action;
			this.Name = "Pearl Diver";
			this.Cost = 2;
			this.Actions = 1;
			this.Cards = 1;
		}

		private static string[] choices = new string[] { "Take", "Leave" };
		private static string[] choiceDescriptions = new string[] { "Yes", "No"};
		public override void Play(GameModel gameModel)
		{
			CardModel card = gameModel.CurrentPlayer.DrawCardFromBottom();
			if (card != null)
			{
				gameModel.TextLog.WriteLine(gameModel.CurrentPlayer.Name + " reveals " + card.Name);
				int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.PearlDiver, card, "Do you want to put it on top of your deck?", choices, choiceDescriptions);
				if (choice == 0)
				{
					gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
				}
				else
				{
					gameModel.CurrentPlayer.Deck.PlaceOnBottom(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new PearlDiver();
		}
	}
}