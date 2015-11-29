using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Salvager : SeasideCardModel
	{
		public Salvager()
		{
			this.Type = CardType.Action;
			this.Name = "Salvager";
			this.Cost = 4;
			this.Buys = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForSalvager, "Choose a card to trash with Salvager", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (card != null)
			{
				gameModel.CurrentPlayer.AddActionCoin(gameModel.GetCost(card));
				gameModel.CurrentPlayer.Trash(card);
			}
		}

		public override CardModel Clone()
		{
			return new Salvager();
		}
	}
}