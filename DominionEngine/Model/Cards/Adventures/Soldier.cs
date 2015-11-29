using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Soldier : AdventuresCardModel
	{
		public Soldier()
		{
			this.Name = "Soldier";
			this.Type = CardType.Action | CardType.Attack | CardType.Traveller;
			this.Cost = 3;

			this.Coins = 2;
		}

		public override void Play(GameModel gameModel)
		{
			int attacksInPlay = gameModel.CurrentPlayer.Played.Count(c => c.Is(CardType.Attack));
			gameModel.CurrentPlayer.AddActionCoin(attacksInPlay);
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach(Player player in attackedPlayers)
			{
				if(player.Hand.Count >= 4)
				{
					player.DiscardCards(1);
				}
			}
		}
		private static string[] exchangeChoices = new string[] { "Exchange", "Keep" };
		
		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ExchangeSoldierForFugitive, "You may exchange Soldier for a Fugitive", exchangeChoices, exchangeChoices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.Played.Remove(this);
				gameModel.PileMap[typeof(Soldier)].PutCardOnPile(this);
				gameModel.CurrentPlayer.GainCard(typeof(Fugitive));
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
			return new Soldier();
		}
	}
}
