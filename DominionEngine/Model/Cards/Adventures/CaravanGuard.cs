using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class CaravanGuard : AdventuresCardModel
	{
		public CaravanGuard()
		{
			this.Name = "Caravan Guard";
			this.Type = CardType.Action | CardType.Duration | CardType.Reaction;
			this.Cost = 3;
			
			this.Actions = 1;
			this.Cards = 1;
		}

		public override CardModel Clone()
		{
			return new CaravanGuard();
		}

		private Player playedOutOfTurn;
		public override void Play(GameModel gameModel)
		{
			Player player = playedOutOfTurn != null ? playedOutOfTurn : gameModel.CurrentPlayer;
			player.DeferredCoin++;
			playedOutOfTurn = null;
		}

		public override bool ReactToAttack(GameModel gameModel, Player targetPlayer)
		{
			CardModel choice = targetPlayer.Chooser.ChooseZeroOrOneCard(Chooser.CardChoiceType.PlayCaravanGuard, "You may play Caravan Guard", Chooser.ChoiceSource.FromHand, new CardModel[] { this });
			if (choice != null)
			{
				this.playedOutOfTurn = targetPlayer;
				targetPlayer.Play(choice, false, true);
			}
			return false;
		}
	}
}
