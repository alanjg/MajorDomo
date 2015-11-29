using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Rats : DarkAgesCardModel
	{
		public Rats()
		{
			this.Name = "Rats";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Cards = 1;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Rats));
			IEnumerable<CardModel> nonRats = gameModel.CurrentPlayer.Hand.Where(c => !(c is Rats));
			if(!nonRats.Any())
			{
				gameModel.CurrentPlayer.RevealHand();
			}
			else
			{
				CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashFromHand, "Trash a card from your hand other than a Rats", Chooser.ChoiceSource.FromHand, nonRats);
				gameModel.CurrentPlayer.Trash(choice);
			}
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			owner.Draw();
		}

		public override CardModel Clone()
		{
			return new Rats();
		}
	}
}