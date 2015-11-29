using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class FortuneTeller : CornucopiaCardModel
	{
		public FortuneTeller()
		{
			this.Name = "Fortune Teller";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 3;
			this.Coins = 2;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				List<CardModel> discards = new List<CardModel>();
				CardModel top = player.DrawCard();
				while (top != null && !top.Is(CardType.Victory) && !top.Is(CardType.Curse))
				{
					discards.Add(top);
					top = player.DrawCard();
				}
				if (top != null)
				{
					player.Deck.PlaceOnTop(top);
				}
				foreach (CardModel discard in discards)
				{
					player.DiscardCard(discard);
				}
			}
		}

		public override CardModel Clone()
		{
			return new FortuneTeller();
		}
	}
}
