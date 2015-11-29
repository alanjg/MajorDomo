using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Sage : DarkAgesCardModel
	{
		public Sage()
		{
			this.Name = "Sage";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> revealed = new List<CardModel>();
			for(;;)
			{
				CardModel top = gameModel.CurrentPlayer.DrawCard();
				if (top == null)
				{
					break;
				}
				if (gameModel.GetCost(top) >= 3)
				{
					gameModel.CurrentPlayer.PutInHand(top);
					break;
				}
				else
				{
					revealed.Add(top);
				}
			}

			foreach (CardModel card in revealed)
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}			
		}

		public override CardModel Clone()
		{
			return new Sage();
		}
	}
}