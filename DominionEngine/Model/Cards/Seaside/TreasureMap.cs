using System;
using System.Linq;

namespace Dominion.Model.Actions
{
	public class TreasureMap : SeasideCardModel
	{
		public TreasureMap()
		{
			this.Type = CardType.Action;
			this.Name = "Treasure Map";
			this.Cost = 4;
		}
		
		public override void Play(GameModel gameModel)
		{
			CardModel otherMap = gameModel.CurrentPlayer.Hand.FirstOrDefault(card => card is TreasureMap);
			gameModel.CurrentPlayer.Trash(this.ThisAsTrashTarget);
			if (otherMap != null)
			{
				gameModel.CurrentPlayer.Trash(otherMap);
				for (int i = 0; i < 4; i++)
				{
					gameModel.CurrentPlayer.GainCard(typeof(Gold), GainLocation.TopOfDeck);
				}
			}
		}

		public override CardModel Clone()
		{
			return new TreasureMap();
		}
	}
}