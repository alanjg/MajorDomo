using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Contraband : ProsperityCardModel
	{
		public Contraband()
		{
			this.Type = CardType.Treasure;
			this.Name = "Contraband";
			this.Cost = 5;
			this.Coins = 3;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainBuys(1);
			Pile choice = gameModel.LeftOfCurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Contraband, "Choose a card to contraband", gameModel.SupplyPiles);
			gameModel.TextLog.WriteLine(gameModel.LeftOfCurrentPlayer.Name + " chose " + choice.Name);
			choice.Contrabanded = true;
		}

		public override CardModel Clone()
		{
			return new Contraband();
		}
	}
}