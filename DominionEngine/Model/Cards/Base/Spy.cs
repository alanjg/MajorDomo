using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Spy : BaseCardModel
	{
		public Spy()
		{
			this.Name = "Spy";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 4;
			this.Cards = 1;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			this.PlaySpy(gameModel.CurrentPlayer, gameModel.CurrentPlayer, gameModel);
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				this.PlaySpy(gameModel.CurrentPlayer, player, gameModel);
			}
		}

		private static string[] choices = new string[] { "Keep", "Discard" };
		private void PlaySpy(Player attacker, Player target, GameModel gameModel)
		{
			CardModel card = target.DrawCard();
			if (card != null)
			{
				EffectChoiceType choiceType = attacker == target ? EffectChoiceType.DiscardOrPutOnDeck : EffectChoiceType.ForceDiscardOrPutOnDeck;
				int placeOnDeck = attacker.Chooser.ChooseOneEffect(choiceType, card, target.Name + " reveals " + card.Name, choices, choices);
				if (placeOnDeck == 0)
				{
					gameModel.TextLog.WriteLine(target.Name + " places " + card.Name + " on the top of his deck.");
					target.Deck.PlaceOnTop(card);
				}
				else
				{
					target.DiscardCard(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Spy();
		}
	}
}
