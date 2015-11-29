using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class DameAnna : Knights
	{
		public DameAnna()
		{
			this.Name = "Dame Anna";
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> choices = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.TrashFromHand, "You may trash up to 2 cards from your hand", Chooser.ChoiceSource.FromHand, 0, 2, gameModel.CurrentPlayer.Hand);
			foreach (CardModel card in choices.ToList())
			{
				gameModel.CurrentPlayer.Trash(card);
			}
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new DameAnna();
		}
	}
}