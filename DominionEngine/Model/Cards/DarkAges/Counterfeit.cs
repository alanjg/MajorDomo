using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Counterfeit : DarkAgesCardModel
	{
		public Counterfeit()
		{
			this.Name = "Counterfeit";
			this.Type = CardType.Treasure;
			this.Cost = 5;
			this.Coins = 1;
			this.Buys = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.Counterfeit, "You may play a treasure from hand twice", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Treasure)));
			if (choice != null)
			{
				gameModel.CurrentPlayer.Play(choice, false, true);
				gameModel.CurrentPlayer.Play(choice, false, false, " a second time");
				gameModel.CurrentPlayer.Trash(choice);
			}
		}

		public override bool AffectsTreasurePlayOrder
		{
			get
			{
				return true;
			}
		}

		public override CardModel Clone()
		{
			return new Counterfeit();
		}
	}
}