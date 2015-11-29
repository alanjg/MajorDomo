using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Ambassador : SeasideCardModel
	{
		public Ambassador()
		{
			this.Type = CardType.Action | CardType.Attack;
			this.Name = "Ambassador";
			this.Cost = 3;
		}

		public override CardModel Clone()
		{
			Ambassador clone = (Ambassador)base.Clone();
			clone.returnedCard = this.returnedCard;
			return clone;
		}

		public override void Play(GameModel gameModel)
		{
			this.returnedCard = null;
			Player player = gameModel.CurrentPlayer;
			CardModel choice = player.Chooser.ChooseOneCard(CardChoiceType.Ambassador, "Choose a card to return to the supply", Chooser.ChoiceSource.FromHand, player.Hand);
			if (choice != null)
			{
				Pile supply = gameModel.SupplyPiles.Where(pile => pile.Card.Name == choice.Name).FirstOrDefault();
				if (supply != null)
				{
					List<CardModel> toReturn = new List<CardModel>(player.Hand.Where(card => card.Name == choice.Name).Take(2));
					this.returnedCard = choice.GetType();
					string[] choices = new string[toReturn.Count + 1];
					for (int i = 0; i < choices.Length; i++)
					{
						choices[i] = i.ToString();
					}
					int trashChoice = player.Chooser.ChooseOneEffect(EffectChoiceType.AmbassadorCount, choice, "Return how many to supply?", choices, choices);

					for (int i = 0; i < trashChoice; i++)
					{
						player.RemoveFromHand(toReturn[i]);
						supply.PutCardOnPile(toReturn[i]);
					}
				}
			}
		}

		private Type returnedCard;

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			if (this.returnedCard != null)
			{
				foreach (Player player in attackedPlayers)
				{
					player.GainCard(this.returnedCard);
				}
			}
		}
	}
}