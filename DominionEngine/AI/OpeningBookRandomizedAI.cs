using Dominion;
using Dominion.Model.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionEngine.AI
{
	public sealed class OpeningBookRandomizedAIStrategy : BaseAIStrategy
	{
		private KeyValuePair<Pile, Pile>? opening = null;
		public OpeningBookRandomizedAIStrategy(GameModel gameModel)
			: base(gameModel, new BaseAIChooser(gameModel))
		{
			((BaseAIChooser)this.Chooser).SetStrategy(this);
		}


		
		public override string Name
		{
			get { return "Opening book AI"; }
		}

		protected override PlayerAction BuyPhase()
		{
			if (this.Player.Bought.Count == 0)
			{
				if (this.Player.CoinTokens > 0)
				{
					return new PlayerAction() { ActionType = ActionType.PlayCoinTokens };
				}
				if (this.Player.HasBasicTreasures)
				{
					return new PlayerAction() { ActionType = ActionType.PlayBasicTreasures };
				}

				foreach (CardModel card in this.Player.Hand)
				{
					if (card.Is(CardType.Treasure))
					{
						return new PlayerAction() { ActionType = ActionType.PlayCard, Card = card };
					}
				}
			}
			PlayerAction playerAction = null;
			if (this.Player.TurnCount == 1 || this.Player.TurnCount == 2)
			{
				if (this.opening == null)
				{
					this.opening = OpeningBook.GetOpening(this.Player.Coin == 3 || this.Player.Coin == 4, this.GameModel);
				}
				if (this.opening.Value.Key != null)
				{
					if (this.opening.Value.Value != null)
					{
						if (this.Player.Coin >= 4)
						{
							Pile max = this.opening.Value.Key.GetCost() > this.opening.Value.Value.GetCost() ? this.opening.Value.Key : this.opening.Value.Value;
							if (this.TryBuyCard(max.TopCard.GetType(), out playerAction))
							{
								return playerAction;
							}
						}
						else
						{
							Pile min = this.opening.Value.Key.GetCost() <= this.opening.Value.Value.GetCost() ? this.opening.Value.Key : this.opening.Value.Value;
							if (this.TryBuyCard(min.TopCard.GetType(), out playerAction))
							{
								return playerAction;
							}
						}
					}
					else
					{
						if (this.TryBuyCard(this.opening.Value.Key.TopCard.GetType(), out playerAction))
						{
							return playerAction;
						}
						else
						{
							return new PlayerAction() { ActionType = ActionType.EndTurn };
						}
					}
				}
			}

			if (this.TryBuyCard(typeof(Colony), out playerAction)) { return playerAction; }
			if (this.TryBuyCard(typeof(Platinum), out playerAction)) { return playerAction; }
			if (this.TryBuyCard(typeof(Province), out playerAction)) { return playerAction; }
			GameSegment segment = this.GetGameSegment();
			IEnumerable<Pile> affordable = this.GameModel.SupplyPiles.Where(p => p.Count > 0 && this.GameModel.GetCost(p) <= this.Player.Coin && (this.Player.Potions > 0 || !p.CostsPotion) && !p.Contrabanded && (p.EmbargoCount == 0 || this.GameModel.PileMap[typeof(Curse)].Count == 0) && !(p.Card is Curse) && !(p.Card is Copper) && !(p.Card is Estate) && !p.Card.Is(CardType.Ruins));
			if (segment == GameSegment.Endgame)
			{
				if (this.TryBuyCard(typeof(Duchy), out playerAction)) { return playerAction; }
				foreach (Pile v in affordable.Where(p => p.Card.Is(CardType.Victory)))
				{
					if (this.TryBuyCard(v.Card.GetType(), out playerAction)) { return playerAction; }
				}
				if (this.TryBuyCard(typeof(Estate), out playerAction)) { return playerAction; }
			}
			else if (segment == GameSegment.Early)
			{
				if (this.TryBuyCard(typeof(Gold), out playerAction)) { return playerAction; }
				if (Randomizer.Next(2) == 0 && this.Player.Potions == 0 && this.Player.Coin < 5)
				{
					if (this.TryBuyCard(typeof(Silver), out playerAction)) { return playerAction; }
				}
			}
			else if (segment == GameSegment.Middle)
			{
				if (this.TryBuyCard(typeof(Gold), out playerAction)) { return playerAction; }
				if (Randomizer.Next(2) == 0 && this.Player.Coin < 5)
				{
					if (this.TryBuyCard(typeof(Silver), out playerAction)) { return playerAction; }
				}
			}
			IList<Pile> piles = affordable.OrderBy(p => p, new AIPileComparer(segment, (BaseAIChooser)this.Chooser)).Reverse().ToList();
			if (piles.Count > 0)
			{
				int min = piles.Count;
				for (int i = 0; i < 5; i++)
				{
					min = Math.Min(i, Randomizer.Next(piles.Count));
				}
				if (this.TryBuyCard(piles[min].Card.GetType(), out playerAction)) { return playerAction; }
			}
			return new PlayerAction() { ActionType = ActionType.EndTurn };
		}
	}
}
