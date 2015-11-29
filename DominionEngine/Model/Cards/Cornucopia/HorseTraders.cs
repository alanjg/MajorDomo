using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class HorseTraders : CornucopiaCardModel
	{
		public HorseTraders()
		{
			this.Name = "Horse Traders";
			this.Type = CardType.Action | CardType.Reaction;
			this.Cost = 4;
			this.Buys = 1;
			this.Coins = 3;
			this.ReactionTrigger = ReactionTrigger.AttackPlayed;
		}

		public override void Play(GameModel gameModel)
		{
			Player player = gameModel.CurrentPlayer;
			if (gameModel.CurrentPlayer.Hand.Count == 1)
			{
				player.DiscardCard(gameModel.CurrentPlayer.Hand[0]);
			}
			else if (gameModel.CurrentPlayer.Hand.Count == 2)
			{
				player.DiscardCard(gameModel.CurrentPlayer.Hand[1]);
				player.DiscardCard(gameModel.CurrentPlayer.Hand[0]);
			}
			else if(gameModel.CurrentPlayer.Hand.Count > 2)
			{
				IEnumerable<CardModel> discards = player.Chooser.ChooseSeveralCards(CardChoiceType.Discard, "Discard 2 cards", ChoiceSource.FromHand, 2, 2, player.Hand);
				foreach (CardModel discard in discards.ToArray())
				{
					player.DiscardCard(discard);
				}
			}
		}

		public override bool ReactToAttack(GameModel gameModel, Player targetPlayer)
		{
			targetPlayer.RemoveFromHand(this);
			targetPlayer.SetAsideForHorseTraders(this);
			return false;
		}

		public override CardModel Clone()
		{
			return new HorseTraders();
		}
	}
}
