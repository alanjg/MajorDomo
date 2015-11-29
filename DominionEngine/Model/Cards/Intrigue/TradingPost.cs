using System;
using System.Linq;
using Dominion.Model.Chooser;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class TradingPost : IntrigueCardModel
	{
		public TradingPost()
		{
			this.Type = CardType.Action;
			this.Name = "Trading Post";
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			Player currentPlayer = gameModel.CurrentPlayer;
			IList<CardModel> trashed = currentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.TrashFromHand, "Trash two cards", ChoiceSource.FromHand, 2, 2, currentPlayer.Hand).ToList();
			foreach (CardModel trash in trashed)
			{
				currentPlayer.Trash(trash);
			}
			if (trashed.Count == 2)
			{
				currentPlayer.GainCard(typeof(Silver), GainLocation.InHand);
			}
		}

		public override CardModel Clone()
		{
			return new TradingPost();
		}
	}
}
