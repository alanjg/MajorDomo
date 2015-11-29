using System;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Minion : IntrigueCardModel
	{
		private bool playAttack = false;
		public Minion()
		{
			this.Name = "Minion";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
			this.Actions = 1;
		}

		public override CardModel Clone()
		{
			Minion clone = (Minion)base.Clone();
			clone.playAttack = this.playAttack;
			return clone;
		}

		private static string[] choices = new string[] { "Coin", "Discard" };
		private static string[] choiceDescriptions = new string[] { "+2 Coin", "Discard hand and draw 4 cards" };

		public override void Play(GameModel gameModel)
		{
			this.playAttack = false;
			Player currentPlayer = gameModel.CurrentPlayer;
			int choice = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Minion, "Minion", choices, choiceDescriptions);
			if (choice == 0)
			{
				currentPlayer.AddActionCoin(2);
			}
			else
			{
				currentPlayer.DiscardHand();
				currentPlayer.Draw(4);
				this.playAttack = true;
			}
		}

		public override void PlayAttack(GameModel gameModel, System.Collections.Generic.IEnumerable<Player> attackedPlayers)
		{
			if (this.playAttack)
			{
				foreach (Player otherPlayer in attackedPlayers)
				{
					if (otherPlayer.Hand.Count > 4)
					{
						otherPlayer.DiscardHand();
						otherPlayer.Draw(4);
					}
				}
			}
		}
	}
}
