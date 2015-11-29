using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class TradeRoute : ProsperityCardModel
	{
		public TradeRoute()
		{
			this.Type = CardType.Action;
			this.Name = "Trade Route";
			this.Cost = 3;
			this.Buys = 1;
		}
		
		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddActionCoin(gameModel.TradeRouteCount);
			if (gameModel.CurrentPlayer.Hand.Count > 0)
			{
				CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashFromHand, "Choose a card to trash with Trade Route", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				if (choice != null)
				{
					gameModel.CurrentPlayer.Trash(choice);
				}
			}
		}

		public override CardModel Clone()
		{
			return new TradeRoute();
		}
	}
}