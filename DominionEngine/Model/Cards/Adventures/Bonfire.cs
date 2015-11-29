using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Bonfire : AdventuresCardModel
	{
		public Bonfire()
		{
			this.Name = "Bonfire";
			this.Type = CardType.Event;
			this.Cost = 3;
		}

		public override void OnBuy(GameModel gameModel)
		{
			foreach(CardModel card in gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(Chooser.CardChoiceType.Trash, "Trash up to 2 cards you have in play", Chooser.ChoiceSource.InPlay, 0, 2, gameModel.CurrentPlayer.Played).ToArray())
			{
				gameModel.CurrentPlayer.Trash(card);
			}
		}

		public override CardModel Clone()
		{
			return new Bonfire();
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}
	}
}
