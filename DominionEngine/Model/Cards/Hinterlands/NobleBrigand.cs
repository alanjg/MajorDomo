using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class NobleBrigand : HinterlandsCardModel
	{
		public NobleBrigand()
		{
			this.Name = "Noble Brigand";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 4;
			this.Coins = 1;
		}

		private void DoAttack(Player attacker, Player attacked)
		{
			List<CardModel> cards = attacked.DrawCards(2);

			bool hadTreasure = cards.Any(c => c.Is(CardType.Treasure));
			IEnumerable<CardModel> gainable = cards.Where(c => c is Silver || c is Gold);
			
			if (gainable.Any())
			{
				CardModel treasure = attacker.Chooser.ChooseOneCard(CardChoiceType.TrashForNobleBrigand, "Trash a treasure", ChoiceSource.None, gainable);
				if (treasure != null)
				{
					attacked.Trash(treasure);
					attacker.GainCard(treasure, null);
					cards.Remove(treasure);
				}
			}

			foreach (CardModel other in cards)
			{
				attacked.DiscardCard(other);
			}

			if (!hadTreasure)
			{
				attacked.GainCard(typeof(Copper));
			}
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player attacked in attackedPlayers)
			{
				this.DoAttack(gameModel.CurrentPlayer, attacked);
			}
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			foreach (Player otherPlayer in gameModel.Players)
			{
				if (otherPlayer != player)
				{
					this.DoAttack(player, otherPlayer);
				}
			}
		}

		public override CardModel Clone()
		{
			return new NobleBrigand();
		}
	}
}
