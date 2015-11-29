using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Forager : DarkAgesCardModel
	{
		public Forager()
		{
			this.Name = "Forager";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Actions = 1;
			this.Buys = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashFromHand, "Trash a card from your hand", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);	
			if (choice != null)
			{
				gameModel.CurrentPlayer.Trash(choice);
			}

			int count = gameModel.Trash.Where(card => card.Is(CardType.Treasure)).Select(card => card.Name).Distinct().Count();
			gameModel.CurrentPlayer.AddActionCoin(count);
		}

		public override CardModel Clone()
		{
			return new Forager();
		}
	}
}