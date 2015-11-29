using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Golem : AlchemyCardModel
	{
		public Golem()
		{
			this.Name = "Golem";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.CostsPotion = true;
		}

		public override void Play(GameModel gameModel)
		{
			List<CardModel> actions = new List<CardModel>();
			List<CardModel> discards = new List<CardModel>();
			Player player = gameModel.CurrentPlayer;
			while (actions.Count != 2)
			{
				CardModel card = player.DrawCard();
				if (card != null)
				{
					player.RevealCard(card);
					if (card.Is(CardType.Action) && !(card is Golem))
					{
						actions.Add(card);
					}
					else
					{
						discards.Add(card);
					}
				}
				else
				{
					break;
				}	
			}

			foreach (CardModel card in discards)
			{
				player.DiscardCard(card);
			}

			if (actions.Count == 2)
			{
				CardModel choice = player.Chooser.ChooseOneCard(CardChoiceType.GolemPlayOrder, "Play first action", Chooser.ChoiceSource.None, actions);
				player.Play(choice, false, true);
				actions.Remove(choice);
				player.Play(actions[0], false, true);
			}
			else if (actions.Count == 1)
			{
				player.Play(actions[0], false, true);
			}
		}

		public override CardModel Clone()
		{
			return new Golem();
		}
	}
}
