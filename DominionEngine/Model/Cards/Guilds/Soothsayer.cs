using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Soothsayer : GuildsCardModel
	{
		public Soothsayer()
		{
			this.Name = "Soothsayer";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Gold));
		}

		public override void PlayAttack(GameModel gameModel, System.Collections.Generic.IEnumerable<Player> attackedPlayers)
		{
			Pile curses = gameModel.PileMap[typeof(Curse)];
			foreach (Player p in attackedPlayers)
			{
				int curseCount = curses.Count;
				p.GainCard(curses);
				if (curses.Count != curseCount)
				{
					p.Draw(1);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Soothsayer();
		}
	}
}
