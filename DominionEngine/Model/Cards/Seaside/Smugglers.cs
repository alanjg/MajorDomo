using System;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Smugglers : SeasideCardModel
	{
		public Smugglers()
		{
			this.Type = CardType.Action;
			this.Name = "Smugglers";
			this.Cost = 3;
		}

		public override void Play(GameModel gameModel)
		{
			int previousPlayerIndex = (gameModel.Players.IndexOf(gameModel.CurrentPlayer) - 1 + gameModel.Players.Count) % gameModel.Players.Count;
			Player previousPlayer = gameModel.Players[previousPlayerIndex];
			Player player = gameModel.CurrentPlayer;

			Pile choice = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing up to 6 that was gained last turn", Smugglers.SmugglerChoices(gameModel));
				
			if (choice != null)
			{
				player.GainCard(choice);
			}
		}

		public static IEnumerable<Pile> SmugglerChoices(GameModel gameModel)
		{
			return gameModel.SupplyPiles.Where(p => p.Count > 0 && gameModel.RightOfCurrentPlayer.GainedLastTurn.Any(c => c.Name == p.TopCard.Name) && p.Cost <= 6 && !p.CostsPotion);
		}

		public override CardModel Clone()
		{
			return new Smugglers();
		}
	}
}