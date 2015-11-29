using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Bishop : ProsperityCardModel
	{
		private static string[] choices = new string[] { "Trash", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Trash a card from hand", "Do nothing" };
		public Bishop()
		{
			this.Type = CardType.Action;
			this.Name = "Bishop";
			this.Cost = 4;
			this.Coins = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddVPChips(1);
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForBishop, "Trash a card", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (choice != null)
			{
				gameModel.CurrentPlayer.Trash(choice);
				int cost = gameModel.GetCost(choice);
				gameModel.CurrentPlayer.AddVPChips(cost / 2);
			}
			foreach (Player player in gameModel.Players)
			{
				if (player != gameModel.CurrentPlayer)
				{
					CardModel trashed = player.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashFromHand, "You may trash a card", Chooser.ChoiceSource.FromHand, player.Hand);
					if (trashed != null)
					{
						player.Trash(trashed);
					}
				}
			}
		}

		public override CardModel Clone()
		{
			return new Bishop();
		}
	}
}