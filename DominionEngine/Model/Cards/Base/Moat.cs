using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Moat : BaseCardModel
	{
		public Moat()
		{
			this.Name = "Moat";
			this.Type = CardType.Action | CardType.Reaction;
			this.Cards = 2;
			this.Cost = 2;
			this.ReactionTrigger = ReactionTrigger.AttackPlayed;
		}

		public override bool ReactToAttack(GameModel gameModel, Player targetPlayer)
		{
			return true;
		}

		public override CardModel Clone()
		{
			return new Moat();
		}
	}
}
