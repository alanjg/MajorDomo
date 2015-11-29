using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Alchemist : AlchemyCardModel
	{
		public Alchemist()
		{
			this.Name = "Alchemist";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.CostsPotion = true;
			this.Actions = 1;
			this.Cards = 2;
		}

		public override void OnCleanup(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromCleanUp(this);
			gameModel.CurrentPlayer.Deck.PlaceOnTop(this);
			gameModel.TextLog.WriteLine("Returning an Alchemist to the top of the deck.");
		}

		public override bool HasCleanupEffect(GameModel gameModel)
		{
			return gameModel.CurrentPlayer.Played.Any(card => card is Potion);
		}

		public override CardModel Clone()
		{
			return new Alchemist();
		}
	}
}
