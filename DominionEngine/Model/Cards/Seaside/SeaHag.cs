using System;
using System.Linq;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class SeaHag : SeasideCardModel
	{
		public SeaHag()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Sea Hag";
			this.Cost = 4;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				CardModel card = player.DrawCard();
				if (card != null)
				{
					player.DiscardCard(card);
				}
				player.GainCard(typeof(Curse), GainLocation.TopOfDeck);
			}
		}

		public override CardModel Clone()
		{
			return new SeaHag();
		}
	}
}