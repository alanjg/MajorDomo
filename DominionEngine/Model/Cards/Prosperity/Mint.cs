using System;
using System.Linq;
using System.Net;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Mint : ProsperityCardModel
	{
		public Mint()
		{
			this.Type = CardType.Action;
			this.Name = "Mint";
			this.Cost = 5;
		}

		public override bool AffectsTreasurePlayOrder
		{
			get
			{
				return true;
			}
		}

		public override void Play(GameModel gameModel)
		{
			CardModel chosen = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.Gain, "You may gain a copy of a treasure", Chooser.ChoiceSource.FromHand, (gameModel.CurrentPlayer.Hand.Where(card => card.Is(CardType.Treasure))));
			if (chosen != null)
			{
				gameModel.CurrentPlayer.GainCard(chosen.GetType());
			}
		}

		public override void OnBuy(GameModel gameModel)
		{
			foreach (CardModel treasure in gameModel.CurrentPlayer.Played.Where(playedCard => playedCard.Is(CardType.Treasure)).ToList())
			{
				gameModel.CurrentPlayer.Trash(treasure);
			}
		}

		public override CardModel Clone()
		{
			return new Mint();
		}
	}
}