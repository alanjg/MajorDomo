using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Menagerie : CornucopiaCardModel
	{
		public Menagerie()
		{
			this.Name = "Menagerie";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RevealHand();
			
			if (gameModel.CurrentPlayer.Hand.Select(card => card.Name).Distinct().Count() == gameModel.CurrentPlayer.Hand.Count)
			{
				gameModel.CurrentPlayer.Draw(3);
			}
			else
			{
				gameModel.CurrentPlayer.Draw();
			}
		}

		public override CardModel Clone()
		{
			return new Menagerie();
		}
	}
}
