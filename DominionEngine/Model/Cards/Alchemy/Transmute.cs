using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Transmute : AlchemyCardModel
	{
		public Transmute()
		{
			this.Name = "Transmute";
			this.Cost = 0;
			this.CostsPotion = true;
			this.Type = CardType.Action;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForTransmute, "Trash a card from your hand", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (choice != null)
			{
				gameModel.CurrentPlayer.Trash(choice);
				if (choice.Is(CardType.Action))
				{
					gameModel.CurrentPlayer.GainCard(typeof(Duchy));
				}
				if (choice.Is(CardType.Treasure))
				{
					gameModel.CurrentPlayer.GainCard(typeof(Transmute));
				}
				if (choice.Is(CardType.Victory))
				{
					gameModel.CurrentPlayer.GainCard(typeof(Gold));
				}
			}
		}

		public override CardModel Clone()
		{
			return new Transmute();
		}
	}
}
