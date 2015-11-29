using System;
using System.Linq;
using Dominion.Model.Chooser;


namespace Dominion.Model.Actions
{
	public class Explorer : SeasideCardModel
	{
		public Explorer()
		{
			this.Type = CardType.Action;
			this.Name = "Explorer";
			this.Cost = 5;
		}

		private static string[] choices = new string[] { "Reveal", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Reveal Province", "Do Nothing" };
		public override void Play(GameModel gameModel)
		{
			CardModel card = gameModel.CurrentPlayer.Hand.FirstOrDefault(handCard => handCard is Province);
			if (card != null)
			{
				int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Explorer, "You may reveal a province", choices, choiceDescriptions);
				if (choice == 0)
				{
					gameModel.CurrentPlayer.RevealCardFromHand(card);
					gameModel.CurrentPlayer.GainCard(typeof(Gold), GainLocation.InHand);
				}
				else
				{
					gameModel.CurrentPlayer.GainCard(typeof(Silver), GainLocation.InHand);
				}
			}
			else
			{
				gameModel.CurrentPlayer.GainCard(typeof(Silver), GainLocation.InHand);
			}
		}

		public override CardModel Clone()
		{
			return new Explorer();
		}
	}
}