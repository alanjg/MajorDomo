using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class IllGottenGains : HinterlandsCardModel
	{
		public IllGottenGains()
		{
			this.Name = "Ill-Gotten Gains";
			this.Type = CardType.Treasure;
			this.Cost = 5;
			this.Coins = 1;
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			foreach (Player otherPlayer in gameModel.Players)
			{
				if (player != otherPlayer)
				{
					otherPlayer.GainCard(typeof(Curse));
				}
			}
		}

		private static string[] choices = new string[] { "Yes", "No" };
		public override void Play(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.IllGottenGains, "Gain a copper in hand?", choices, choices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.GainCard(typeof(Copper), GainLocation.InHand);
			}
		}

		public override CardModel Clone()
		{
			return new IllGottenGains();
		}
	}
}
