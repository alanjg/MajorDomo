using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Tournament : CornucopiaCardModel
	{
		public Tournament()
		{
			this.Name = "Tournament";
			this.Cost = 4;
			this.Type = CardType.Action;
			this.Actions = 1;
		}

		private static string[] choices = new string[] { "Reveal", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Reveal Province", "Do nothing" };
		public override void Play(GameModel gameModel)
		{
			bool youRevealed = false;
			bool anyRevealed = false;

			CardModel province = gameModel.CurrentPlayer.Hand.FirstOrDefault(card => card is Province);
			if (province != null)
			{
				int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.RevealForTournament, "You may reveal a province", choices, choiceDescriptions);
				if (choice == 0)
				{
					gameModel.TextLog.WriteLine(gameModel.CurrentPlayer.Name + " reveals a Province");
					youRevealed = true;
				}
			}

			foreach (Player player in gameModel.Players)
			{
				if (player != gameModel.CurrentPlayer)
				{
					CardModel theirProvince = player.Hand.FirstOrDefault(card => card is Province);
					if (theirProvince != null)
					{
						int choice = player.Chooser.ChooseOneEffect(EffectChoiceType.RevealForTournament, "You may reveal a province", choices, choiceDescriptions);
						if (choice == 0)
						{
							gameModel.TextLog.WriteLine(player.Name + " reveals a Province");
							anyRevealed = true;
						}
					}
				}	
			}
			if (youRevealed)
			{
				gameModel.CurrentPlayer.DiscardCard(province);
				List<CardModel> prizes = new List<CardModel>(gameModel.Prizes);
				// gain a prize
				Pile duchies = gameModel.SupplyPiles.First(pile => pile.Card is Duchy);
				if (duchies.Count > 0)
				{
					prizes.Add(new Duchy());
				}

				if (prizes.Count > 0)
				{
					CardModel gainedChoice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.GainOnTopOfDeck, "Gain a prize or a duchy", Chooser.ChoiceSource.None, prizes);
					if (gainedChoice is Duchy)
					{
						gameModel.CurrentPlayer.GainCard(typeof(Duchy), GainLocation.TopOfDeck);
					}
					else
					{
						gameModel.TakePrize(gainedChoice);
						gameModel.CurrentPlayer.GainCard(gainedChoice, null, GainLocation.TopOfDeck);
					}
				}
			}

			if (!anyRevealed)
			{
				gameModel.CurrentPlayer.AddActionCoin(1);
				gameModel.CurrentPlayer.Draw();
			}
		}

		public override CardModel Clone()
		{
			return new Tournament();
		}
	}
}
