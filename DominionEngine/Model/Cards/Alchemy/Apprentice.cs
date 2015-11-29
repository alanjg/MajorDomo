using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Apprentice : AlchemyCardModel
	{
		public Apprentice()
		{
			this.Name = "Apprentice";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			Player player = gameModel.CurrentPlayer;
			CardModel choice = player.Chooser.ChooseOneCard(CardChoiceType.TrashForApprentice, "Trash a card", Chooser.ChoiceSource.FromHand, player.Hand);
			if (choice != null)
			{
				int cost = gameModel.GetCost(choice);
				if (choice.CostsPotion)
				{
					cost += 2;
				} 
						
				player.Trash(choice);
				player.Draw(cost);									
			}
		}

		public override CardModel Clone()
		{
			return new Apprentice();
		}
	}
}
