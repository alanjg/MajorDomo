using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Save : AdventuresCardModel
	{
		public Save()
		{
			this.Name = "Save";
			this.Type = CardType.Event;
			this.Cost = 1;
			
			
			this.Buys = 2;
		}

		public override void OnBuy(GameModel gameModel)
		{
			if (!gameModel.CurrentPlayer.HasUsedSaveEvent)
			{
				CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(Chooser.CardChoiceType.SaveEvent, "Set aside a card from your hand", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
				if (card != null)
				{
					gameModel.CurrentPlayer.SaveEventCard = card;
					gameModel.CurrentPlayer.Hand.Remove(card);
				}
				gameModel.CurrentPlayer.HasUsedSaveEvent = true;
			}
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}

		public override CardModel Clone()
		{
			return new Save();
		}
	}
}
