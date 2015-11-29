using Dominion;
using Dominion.Model.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionEngine.AI
{
	// used to decide which cards to keep in hand.  Cards that compare as greater than are better to keep in hand.
	public class DiscardCardComparer : Comparer<CardModel>
	{
		private BaseAIChooser chooser;
		private GameSegment gameSegment;
		public DiscardCardComparer(BaseAIChooser chooser, GameSegment gameSegment)
		{
			this.chooser = chooser;
			this.gameSegment = gameSegment;
		}

		public override int Compare(CardModel x, CardModel y)
		{
			return DiscardCardComparer.Compare(x, y, true, false, this.chooser.Player, this.gameSegment);
		}

		public static int Compare(CardModel x, CardModel y, bool discard, bool gain, Player player, GameSegment gameSegment)
		{
			bool xIsNonusefulVictory = x.Is(CardType.Victory) && !x.Is(CardType.Action) && !x.Is(CardType.Treasure);
			bool yIsNonusefulVictory = y.Is(CardType.Victory) && !y.Is(CardType.Action) && !y.Is(CardType.Treasure);
			if (x.Name == y.Name) return 0;
			if (xIsNonusefulVictory && yIsNonusefulVictory)
			{
				int xVP = x.GetVictoryPoints(player);
				int yVP = y.GetVictoryPoints(player);
				return xVP - yVP;
			}
			if (discard)
			{
				if (xIsNonusefulVictory && !yIsNonusefulVictory)
				{
					return -1;
				}
				else if (!xIsNonusefulVictory && yIsNonusefulVictory)
				{
					return 1;
				}
			}

			if (x.Is(CardType.Curse)) return -1;
			if (y.Is(CardType.Curse)) return 1;
			if (x.Is(CardType.Ruins)) return -1;
			if (y.Is(CardType.Ruins)) return 1;
			if (x.Is(CardType.Shelter)) return -1;
			if (y.Is(CardType.Shelter)) return 1;

			if (gain)
			{
				switch (gameSegment)
				{
					case GameSegment.Early:
						// avoid victory points early
						if (x.Is(CardType.Victory) && !y.Is(CardType.Victory)) return -1;
						if (!x.Is(CardType.Victory) && y.Is(CardType.Victory)) return 1;
						break;
					case GameSegment.Middle:
						// avoid estates in the mid game
						if (x is Estate && !(y is Estate)) return -1;
						if (!(x is Estate) && y is Estate) return 1;
						break;
					case GameSegment.Endgame:
						// always take victory points late
						if (x.Is(CardType.Victory) && !y.Is(CardType.Victory)) return 1;
						if (!x.Is(CardType.Victory) && y.Is(CardType.Victory)) return -1;
						break;
				}
			}

			if (discard)
			{
				if (x.Is(CardType.Treasure) && y.Is(CardType.Treasure))
				{
					return x.Coins.CompareTo(y.Coins);
				}
			}
			if (gain)
			{
				int result = AIPileComparer.Compare(x, y);
				if (result != 0)
				{
					return result;
				}
			}
			int xCost = x.GetBaseCost();
			int yCost = y.GetBaseCost();
			if (xCost != yCost) return xCost < yCost ? -1 : 1;
			return 0;
		}
	}

	// Used to decide which cards to gain.  Cards that compare as greater than are better to gain.
	public class GainCardComparer : Comparer<CardModel>
	{
		private BaseAIChooser chooser;
		private GameSegment gameSegment;
		public GainCardComparer(BaseAIChooser chooser, GameSegment gameSegment)
		{
			this.chooser = chooser;
			this.gameSegment = gameSegment;
		}

		public override int Compare(CardModel x, CardModel y)
		{
			return DiscardCardComparer.Compare(x, y, false, true, this.chooser.Player, this.gameSegment);
		}
	}
}
