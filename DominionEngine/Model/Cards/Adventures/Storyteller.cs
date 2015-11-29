using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Storyteller : AdventuresCardModel
	{
		public Storyteller()
		{
			this.Name = "Storyteller";
			this.Type = CardType.Action;
			this.Cost = 5;
			
			this.Actions = 1;
			this.Coins = 1;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> cards = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(Chooser.CardChoiceType.PlayTreasuresForStoryteller, "Play up to 3 Treasures from your hand.", Chooser.ChoiceSource.FromHand, 0, 3, gameModel.CurrentPlayer.Hand).ToArray();
			
			foreach(CardModel card in cards)
			{
				gameModel.CurrentPlayer.PlayTreasure(card);
			}

			int coin = gameModel.CurrentPlayer.Coin;
			gameModel.CurrentPlayer.Coin = 0;
			gameModel.TextLog.WriteLine(gameModel.CurrentPlayer.Name + " pays $" + coin);
			gameModel.CurrentPlayer.Draw(coin);
		}

		public override CardModel Clone()
		{
			return new Storyteller();
		}
	}
}
