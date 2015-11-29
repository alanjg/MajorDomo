using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Giant : AdventuresCardModel
	{
		public Giant()
		{
			this.Name = "Giant";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.JourneyTokenIsFaceUp = !gameModel.CurrentPlayer.JourneyTokenIsFaceUp;
			if (!gameModel.CurrentPlayer.JourneyTokenIsFaceUp)
			{
				gameModel.CurrentPlayer.AddActionCoin(1);
			}
			else
			{
				gameModel.CurrentPlayer.AddActionCoin(5);
			}
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			if (gameModel.CurrentPlayer.JourneyTokenIsFaceUp)
			{
				foreach (Player player in attackedPlayers)
				{
					CardModel card = player.DrawCards(1).FirstOrDefault();
					if(card != null)
					{
						if (gameModel.GetCost(card) >= 3 && gameModel.GetCost(card) <= 6 && !card.CostsPotion)
						{
							player.Trash(card);
						}
						else
						{
							player.DiscardCard(card);
							player.GainCard(typeof(Curse));
						}
					}
				}
			}
		}
		public override CardModel Clone()
		{
			return new Giant();
		}
	}
}
