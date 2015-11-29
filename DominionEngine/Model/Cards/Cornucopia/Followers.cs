using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Followers : CornucopiaCardModel
	{
		public Followers()
		{
			this.Name = "Followers";
			this.Type = CardType.Action | CardType.Prize | CardType.Attack;
			this.Cost = 0;
			this.Cards = 2;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Estate));
		}

		public override void  PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				player.GainCard(typeof(Curse));
				player.DiscardTo(3);
			}
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Followers();
		}
	}
}
