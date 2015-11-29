using System;
using Dominion.Model.Chooser;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class PirateShip : SeasideCardModel
	{
		private bool playAttack = false;
		public PirateShip()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Pirate Ship";
			this.Cost = 4;
		}

		public override CardModel Clone()
		{
			PirateShip clone = (PirateShip)base.Clone();
			clone.playAttack = this.playAttack;
			return clone;
		}

		private static string[] choices = new string[] { "Attack", "Coin" };
		private static string[] choiceDescriptions = new string[] { "Attack other players", "Gain treasure coin" };

		public override void Play(GameModel gameModel)
		{
			this.playAttack = false;
			Player currentPlayer = gameModel.CurrentPlayer;
			int choice = currentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.PirateShip, "You have " + currentPlayer.PirateShipTokens + " treasure tokens", choices, choiceDescriptions);
			if (choice == 0)
			{
				this.playAttack = true;
			}
			else
			{
				currentPlayer.AddActionCoin(currentPlayer.PirateShipTokens);
			}
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			if (this.playAttack)
			{
				Player currentPlayer = gameModel.CurrentPlayer;

				bool addTreasureToken = false;

				foreach (Player player in attackedPlayers)
				{
					List<CardModel> treasures = new List<CardModel>();
					List<CardModel> nonTreasures = new List<CardModel>();
					CardModel card1 = player.DrawCard();
					CardModel card2 = player.DrawCard();

					if (card1 != null)
					{
						if (card1.Is(CardType.Treasure))
						{
							treasures.Add(card1);
						}
						else
						{
							nonTreasures.Add(card1);
						}
					}

					if (card2 != null)
					{
						if (card2.Is(CardType.Treasure))
						{
							treasures.Add(card2);
						}
						else
						{
							nonTreasures.Add(card2);
						}
					}

					if (treasures.Count == 2)
					{
						CardModel choice = currentPlayer.Chooser.ChooseOneCard(CardChoiceType.ForceTrash, "Trash a card", ChoiceSource.None, treasures);
						player.Trash(choice);
						player.DiscardCard(choice == card1 ? card2 : card1);
						addTreasureToken = true;
					}
					else if (treasures.Count == 1)
					{
						player.Trash(treasures[0]);
						addTreasureToken = true;
					}
					foreach (CardModel card in nonTreasures)
					{
						player.DiscardCard(card);
					}
				}

				if (addTreasureToken)
				{
					currentPlayer.AddPirateShipTreasureToken();
				}
				this.playAttack = false;
			}
		}
	}
}