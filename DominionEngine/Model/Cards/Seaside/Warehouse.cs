using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Warehouse : SeasideCardModel
	{
		public Warehouse()
		{
			this.Type = CardType.Action;
			this.Name = "Warehouse";
			this.Cost = 3;
			this.Actions = 1;
			this.Cards = 3;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.DiscardCards(3);
		}

		public override CardModel Clone()
		{
			return new Warehouse();
		}
	}
}