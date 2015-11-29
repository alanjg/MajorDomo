using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class BridgeTroll : AdventuresCardModel
	{
		public BridgeTroll()
		{
			this.Name = "Bridge Troll";
			this.Type = CardType.Action | CardType.Duration | CardType.Attack;
			this.Cost = 5;
			this.Buys = 1;
		}

		public override void PlayDuration(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainBuys(1);
			if (!gameModel.CardModifiers.OfType<BridgeTrollCardModifier>().Any(m => m.Source == this))
			{
				gameModel.AddCardModifier(new BridgeTrollCardModifier(this));
			}
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach(Player p in attackedPlayers)
			{
				p.HasMinusOneCoinToken = true;
			}
		}

		private class BridgeTrollCardModifier : CardModifier
		{
			private BridgeTroll source;
			public BridgeTroll Source { get { return this.source; } }
			public BridgeTrollCardModifier(BridgeTroll source)
			{
				this.source = source;
			}

			public override int GetCost(CardModel cardModel, int cost)
			{
				return Math.Max(cost - 1, 0);
			}

			public override CardModifier Clone()
			{
				// Todo, Bridge Troll doesn't clone correctly.
				return new BridgeTrollCardModifier(this.source);
			}
		}

		public override void Play(GameModel gameModel)
		{
			if (!gameModel.CardModifiers.OfType<BridgeTrollCardModifier>().Any(m => m.Source == this))
			{
				gameModel.AddCardModifier(new BridgeTrollCardModifier(this));
			}
		}

		public override void OnRemovedFromPlay(GameModel gameModel)
		{
			gameModel.CardModifiers.RemoveAll(c => c is BridgeTrollCardModifier && ((BridgeTrollCardModifier)c).Source == this);
        }

		public override CardModel Clone()
		{
			return new BridgeTroll();
		}
	}
}
