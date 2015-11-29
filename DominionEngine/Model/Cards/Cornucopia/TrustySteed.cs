using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class TrustySteed : CornucopiaCardModel
	{
		public override bool IsKingdomCard { get { return false; } }

		public TrustySteed()
		{
			this.Name = "Trusty Steed";
			this.Type = CardType.Action | CardType.Prize;
			this.Cost = 0;
		}

		private static string[] choices = new string[] { "Cards", "Coins", "Actions", "Silvers" };
		private static string[] choiceDescriptions = new string[] { "+2 Cards", "+2 Coins", "+2 Actions", "Gain 4 silvers and put deck in the discard" };
		public override void Play(GameModel gameModel)
		{
			IEnumerable<int> c = gameModel.CurrentPlayer.Chooser.ChooseSeveralEffects(EffectChoiceType.TrustySteed, "Choose two:", 2, 2, choices, choiceDescriptions);
			foreach (int choice in c)
			{
				switch (choice)
				{
					case 0:
						gameModel.CurrentPlayer.Draw();
						gameModel.CurrentPlayer.Draw();
						break;
					case 1:
						gameModel.CurrentPlayer.AddActionCoin(2);
						break;
					case 2:
						gameModel.CurrentPlayer.GainActions(2);
						break;
					case 3:
						for (int i = 0; i < 4; i++)
						{
							gameModel.CurrentPlayer.GainCard(typeof(Silver));
						}
						gameModel.CurrentPlayer.PutDeckInDiscard();
						break;
				}
			}
		}

		public override CardModel Clone()
		{
			return new TrustySteed();
		}
	}
}
