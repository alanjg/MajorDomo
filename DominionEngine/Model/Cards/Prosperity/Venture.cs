using System;
using System.Collections.Generic;

namespace Dominion.Model.Actions
{
	public class Venture : ProsperityCardModel
	{
		public Venture()
		{
			this.Type = CardType.Treasure;
			this.Name = "Venture";
			this.Cost = 5;
			this.Coins = 1;
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
			List<CardModel> discards = new List<CardModel>();
			CardModel cardModel = gameModel.CurrentPlayer.DrawCard();
			while (cardModel != null && !cardModel.Is(CardType.Treasure))
			{
				discards.Add(cardModel);
				cardModel = gameModel.CurrentPlayer.DrawCard();
			}
			if (cardModel != null)
			{
				gameModel.CurrentPlayer.PlayTreasure(cardModel);
			}
			foreach (CardModel discard in discards)
			{
				gameModel.CurrentPlayer.DiscardCard(discard);
			}
		}

		public override CardModel Clone()
		{
			return new Venture();
		}
	}
}