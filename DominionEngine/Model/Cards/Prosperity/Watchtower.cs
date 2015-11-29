using System;

namespace Dominion.Model.Actions
{
	public class Watchtower : ProsperityCardModel
	{
		public Watchtower()
		{
			this.Type = CardType.Action | CardType.Reaction;
			this.Name = "Watchtower";
			this.Cost = 3;
			this.ReactionTrigger = Dominion.ReactionTrigger.CardGained;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.DrawTo(6);
		}

		public override CardModel Clone()
		{
			return new Watchtower();
		}
	}
}