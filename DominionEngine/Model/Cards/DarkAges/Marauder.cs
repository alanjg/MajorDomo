using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Marauder : DarkAgesCardModel
	{
		public Marauder()
		{
			this.Name = "Marauder";
			this.Type = CardType.Action | CardType.Attack | CardType.Looter;
			this.Cost = 4;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Spoils));
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				player.GainCard(gameModel.Ruins);
			}
		}

		public override CardModel Clone()
		{
			return new Marauder();
		}
	}
}