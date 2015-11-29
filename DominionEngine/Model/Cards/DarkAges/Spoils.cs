using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Spoils : DarkAgesCardModel
	{
		public Spoils()
		{
			this.Name = "Spoils";
			this.Type = CardType.Treasure;
			this.Coins = 3;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromPlayed(this);
			gameModel.PileMap[typeof(Spoils)].PutCardOnPile(this);
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Spoils();
		}
	}
}