using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Amulet : AdventuresCardModel
	{
		public Amulet()
		{
			this.Name = "Amulet";
			this.Type = CardType.Action | CardType.Duration;
			this.Cost = 3;
		}

		public override void Play(GameModel gameModel)
		{
			this.PlayAmulet(gameModel);
		}

		public override void PlayDuration(GameModel gameModel)
		{
			this.PlayAmulet(gameModel);
		}

		public override CardModel Clone()
		{
			return new Amulet();
		}

		private static string[] amuletChoices = new string[] { "Coin", "Trash", "Silver" };
		private static string[] amuletChoiceDescriptions = new string[] { "+$1", "Trash a card from your hand", "Gain a Silver" };
		private void PlayAmulet(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(Chooser.EffectChoiceType.Amulet, "Choose one", amuletChoices, amuletChoiceDescriptions);
			switch(choice)
			{
				case 0:
					gameModel.CurrentPlayer.AddActionCoin(1);
					break;
				case 1:
					CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(Chooser.CardChoiceType.Trash, "Trash a card from your hand", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
					if(card != null)
					{
						gameModel.CurrentPlayer.Trash(card);
					}
					break;
				case 2:
					gameModel.CurrentPlayer.GainCard(typeof(Silver));
					break;
			}
		}
	}
}
