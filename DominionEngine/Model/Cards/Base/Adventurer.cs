using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Actions;

namespace Dominion.Model.Actions
{
	public class Adventurer : BaseCardModel
	{
		public Adventurer()
		{
			this.Name = "Adventurer";
			this.Type = CardType.Action;
			this.Cost = 6;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> drawn = new List<CardModel>();
			List<CardModel> setAside = new List<CardModel>();
			do
			{
				CardModel card = gameModel.CurrentPlayer.DrawCard();
				if (card == null)
				{
					break;
				}
				if (card.Is(CardType.Treasure))
				{
					drawn.Add(card);
				}
				else
				{
					setAside.Add(card);					
				}
			} while (drawn.Count < 2);
			
			foreach(CardModel card in drawn)
			{
				gameModel.CurrentPlayer.PutInHand(card);
			}
			foreach (CardModel card in setAside)
			{
				gameModel.CurrentPlayer.DiscardCard(card);
			}
		}

		public override CardModel Clone()
		{
			return new Adventurer();
		}
	}
}
