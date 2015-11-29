using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class JunkDealer : DarkAgesCardModel
	{
		public JunkDealer()
		{
			this.Name = "Junk Dealer";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Cards = 1;
			this.Actions = 1;
			this.Coins = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashFromHand, "Trash a card from your hand", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (choice != null)
			{
				gameModel.CurrentPlayer.Trash(choice);
			}
		}

		public override CardModel Clone()
		{
			return new JunkDealer();
		}
	}
}