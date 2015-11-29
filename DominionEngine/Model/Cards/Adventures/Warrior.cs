using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Warrior : AdventuresCardModel
	{
		public Warrior()
		{
			this.Name = "Warrior";
			this.Type = CardType.Action | CardType.Traveller | CardType.Attack;
			this.Cost = 4;
			
			this.Cards = 2;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			for (int i = 0; i < gameModel.CurrentPlayer.Played.Count(c => c.Is(CardType.Traveller)) + 1; i++)
			{
				foreach (Player player in attackedPlayers)
				{
					CardModel topCard = player.DrawCard();
					int cost = gameModel.GetCost(topCard);
					if ((cost == 3 || cost == 4) && !topCard.CostsPotion)
					{
						player.Trash(topCard);
					}
					else
					{
						player.DiscardCard(topCard);
					}
				}
			}
		}

		private static string[] exchangeChoices = new string[] { "Exchange", "Keep" };
		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ExchangeWarriorForHero, "You may exchange Warrior for a Hero", exchangeChoices, exchangeChoices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.Played.Remove(this);
				gameModel.PileMap[typeof(Warrior)].PutCardOnPile(this);
				gameModel.CurrentPlayer.GainCard(typeof(Hero));
			}
		}
		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}

		public override CardModel Clone()
		{
			return new Warrior();
		}
	}
}
