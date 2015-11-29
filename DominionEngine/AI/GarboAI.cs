using Dominion;
using Dominion.Model.Actions;
using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DominionEngine.AI
{
	public abstract class GarboBaseStrategy : PlayerStrategy
	{
		protected abstract double[] Weights
		{
			get;
		}

		private class GarboChooser : BaseAIChooser
		{
			private GarboBaseStrategy parent;

			public GarboChooser(GameModel gameModel)
				:base(gameModel)
			{
			}

			public GarboBaseStrategy Parent
			{
				get { return this.parent; }
				set { this.parent = value; }
			}

			public override IEnumerable<CardModel> ChooseCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices)
			{
				switch(choiceType)
				{
					case CardChoiceType.Discard:
						List<CardModel> choiceList = new List<CardModel>(choices);
						List<CardModel> discardList = new List<CardModel>();
						return choiceList.OrderBy(choice => this.parent.StaticEvalSimple(choice, this.parent.GetGamePhase())).Take(minChoices);
				}
				return base.ChooseCards(choiceType, cardInfo, choiceText, source, minChoices, maxChoices, choices);
			}
		}

		protected enum Values : int
		{
			coinValue = 0,
			actionValue,
			cardValue,
			ActionValue,
			combinedValue,
			openingPointValue,
			endgameCoinValue,
			endgamePointValue,
			moatValue,
			buyValue,
			bureaucratValue,

			sentinelEndValue
		}

		public GarboBaseStrategy(GameModel gameModel)
			: base(gameModel, new GarboChooser(gameModel))
		{
			((GarboChooser)this.Chooser).Parent = this;
		}

		protected double GetGamePhase()
		{
			double result = 0.0;
			int somewhatEmptyPiles = 0, nearlyEmptyPiles = 0;
			foreach (Pile pile in (from pile in this.GameModel.SupplyPiles select pile))
			{
				if (pile.Card is Province)
				{
					result = Math.Max(result, 1.0 - (pile.Count / 8.0));
				}
				else
				{
					if (pile.Count < 2) nearlyEmptyPiles++;
					else if (pile.Count < 5) somewhatEmptyPiles++;
				}
			}

			// 3 nearly empty piles is 0.9 endgame
			nearlyEmptyPiles = Math.Min(3, nearlyEmptyPiles);
			somewhatEmptyPiles = Math.Min(3 - nearlyEmptyPiles, somewhatEmptyPiles);

			result = Math.Max(result, (nearlyEmptyPiles * 0.33) + (somewhatEmptyPiles * 0.15));

			Debug.Assert(result >= 0 && result <= 1.0);

			return result;
		}

		public override PlayerAction GetNextAction()
		{
			switch (this.GameModel.CurrentPhase)
			{
				case GamePhase.Action:
					return this.ActionPhase(this.GameModel.CurrentPlayer);
				case GamePhase.Buy:
					return this.BuyPhase(this.GameModel.CurrentPlayer);
			}
			return new PlayerAction();
		}

		// ideas:
		// - card correlation?  combos being the best idea here
		// - optimizing the "good" hand probability
		// total deck optimization?
		protected double EvaulateHand(List<CardModel> cards, double gamePhase)
		{
			int actionCount = 0, buyCount = 0, coinCount = 0, cardCount = 0, pointCount = 0;
			foreach (CardModel card in cards)
			{
				actionCount += card.Actions;
				buyCount += card.Buys;
				coinCount += this.GameModel.GetCoins(card);
				cardCount += card.Cards;
				pointCount += card.GetVictoryPoints(this.Player);
			}

			// Opening -> let's try to go through the deck as much as possible...
			double openingValue = actionCount * 2 + buyCount + coinCount * 3 + cardCount * 4;

			// Endgame -> we want tons of cash!
			double endgameValue = coinCount * 3 + actionCount * 1.75 + buyCount * 1 + cardCount * 3 + (pointCount * 4);

			return (1.0 - gamePhase) * openingValue + gamePhase * endgameValue;
		}

		protected PlayerAction ActionPhase(Player player)
		{
			IEnumerable<CardModel> availableActions = this.GetAvailableActions(player);
			HashSet<CardModel> playedActions = new HashSet<CardModel>();

			while (player.Actions > 0 && availableActions.Count(card => !playedActions.Contains(card)) > 0)
			{
				// Always increase our action count if possible
				CardModel actionCard = (from card in availableActions
										where card.Actions > 0
										select card).FirstOrDefault();

				if (actionCard != null)
				{
					this.GameModel.Play(actionCard);
					availableActions = this.GetAvailableActions(player);
					continue;
				}

				Debug.Assert(player.Actions > 0 && player.Actions < 100);

				// Pick the best card to play.  If we have a bunch of actions, let's try to pick up a bunch more cards.
				CardModel bestCard = null;
				bestCard = (from card in availableActions
							orderby card.Actions
							where card.Actions > 0
							where !playedActions.Contains(card)
							select card).FirstOrDefault();

				if (bestCard == null && player.Actions >= 2)
				{
					// Try to gain cards
					bestCard = (from card in availableActions
								orderby card.Cards descending
								where card.Cards > 0
								where !playedActions.Contains(card)
								select card).FirstOrDefault();
				}

				if (bestCard == null)
				{
					// Try to gain gold?
					bestCard = (from card in availableActions
								orderby this.GameModel.GetCoins(card) descending
								where this.GameModel.GetCoins(card) > 0
								where !playedActions.Contains(card)
								select card).FirstOrDefault();
				}

				if (bestCard == null)
				{
					bestCard = (from card in availableActions
								where !playedActions.Contains(card)
								select card).FirstOrDefault();
				}

				if (bestCard != null)
				{
					playedActions.Add(bestCard);
					return new PlayerAction() { ActionType = Dominion.ActionType.PlayCard, Card = bestCard };
					//		availableActions = this.GetAvailableActions(player);
				}
			}
			return new PlayerAction();
		}

		protected PlayerAction BuyPhase(Player player)
		{
			// play all treasures
			foreach (CardModel card in player.Hand.ToList())
			{
				if (card.Is(CardType.Treasure))
				{
					player.PlayTreasure(card);
				}
			}
			List<CardModel> deck = player.Hand.Union(player.Discard.Union(player.Played)).ToList();
			IEnumerable<Pile> availableBuys = (from pile in this.GameModel.SupplyPiles where pile.Count > 0 && this.GameModel.GetCost(pile) <= player.Coin && (!pile.CostsPotion || player.Potions > 0) select pile);
			double gamePhase = this.GetGamePhase();

			while (player.Buys > 0)
			{
				Pile bestBuy = null;
				List<double> evals = new List<double>();
				List<double> handEvals = new List<double>();
				double bestScore = double.MinValue;

				foreach (Pile pile in availableBuys)
				{
					if (pile.Count == 0 || this.GameModel.GetCost(pile) > player.Coin || (pile.CostsPotion && player.Potions == 0)) continue;

					double eval = this.StaticEvalSimple(pile.Card, gamePhase);
					if (eval > bestScore)
					{
						bestScore = eval;
						bestBuy = pile;
					}

					evals.Add(this.StaticEvalSimple(pile.Card, gamePhase));

					double handEval = 0;
					for (int i = 0; i < 10; i++)
					{
						List<CardModel> cards = new List<CardModel>();
						cards.Add(pile.Card);
						for (int j = 0; j < 4; j++)
						{
							cards.Add(deck[((i + j) * 153431) % deck.Count]);
						}
						handEval += this.EvaulateHand(cards, gamePhase);
					}
					handEvals.Add(handEval);
				}

				double minEval = double.MaxValue, maxEval = double.MinValue;
				double minHandEval = double.MaxValue, maxHandEval = double.MinValue;

				foreach (double eval in evals)
				{
					minEval = Math.Min(eval, minEval);
					maxEval = Math.Max(eval, maxEval);
				}

				foreach (double eval in handEvals)
				{
					minHandEval = Math.Min(eval, minHandEval);
					maxHandEval = Math.Max(eval, maxHandEval);
				}

				int index = 0;
				foreach (Pile pile in availableBuys)
				{
					if (pile.Count == 0 || this.GameModel.GetCost(pile) > player.Coin || (pile.CostsPotion && this.Player.Potions == 0)) continue;

					double score = 0;
					score += ((evals[index] - minEval) / (maxEval - minEval));
					//					score += ((handEvals[index] - minHandEval) / (maxHandEval - minHandEval));

					if (score > bestScore && evals[index] > 0.0)
					{
						bestScore = score;
						bestBuy = pile;
					}

					index++;
				}

				if (bestBuy == null)
				{
					break;
				}
				return new PlayerAction() { ActionType = Dominion.ActionType.BuyCard, Pile = bestBuy };
			}
			return new PlayerAction();
		}

		protected double StaticEvalSimple(CardModel card, double gamePhase)
		{
			int actionCount = 0, buyCount = 0, coinCount = 0, cardCount = 0;
			foreach (CardModel deckCard in this.Player.Hand.Union(this.Player.Discard.Union(this.Player.Played)))
			{
				actionCount += deckCard.Actions;
				buyCount += deckCard.Buys;
				coinCount += this.GameModel.GetCoins(deckCard);
				cardCount += deckCard.Cards;
			}

			double openingValue = -1;
			double endgameValue = -0.5;

			openingValue += (card.Actions * card.Actions) * Lerp(this.Weights[(int)Values.actionValue], this.Weights[(int)Values.actionValue] + 0.15, 3, 10, actionCount, true) +
				(card.Buys * card.Buys) * this.Weights[(int)Values.buyValue] +
				card.Cards * Lerp(this.Weights[(int)Values.cardValue], this.Weights[(int)Values.cardValue] + 0.75, 5, 10, cardCount, true) +
				(this.GameModel.GetCoins(card) * this.GameModel.GetCoins(card)) * Lerp(this.Weights[(int)Values.coinValue], this.Weights[(int)Values.coinValue] + 0.3, 5, 20, coinCount, true) +
				(card.Is(CardType.Action) ? this.Weights[(int)Values.ActionValue] : 0) +
				(card.Actions + card.Cards + this.GameModel.GetCoins(card)) * this.Weights[(int)Values.combinedValue] +
				(card.GetVictoryPoints(this.Player) * this.Weights[(int)Values.openingPointValue]);

			if (card.GetVictoryPoints(this.Player) >= 6)
			{
				// Essentially infinite value.
				openingValue += 30;
			}

			if (card is Bureaucrat)
			{
				openingValue += Lerp(0, this.Weights[(int)Values.bureaucratValue], 5, 20, coinCount, true);
			}

			if (card is Moat)
			{
				openingValue += this.Weights[(int)Values.moatValue];
			}

			endgameValue += card.GetVictoryPoints(this.Player) * card.GetVictoryPoints(this.Player) * card.GetVictoryPoints(this.Player) * this.Weights[(int)Values.endgamePointValue];
			endgameValue += this.GameModel.GetCoins(card) * this.GameModel.GetCoins(card) * this.Weights[(int)Values.endgameCoinValue];

			return (1.0 - gamePhase) * openingValue + gamePhase * endgameValue;
		}

		private IEnumerable<CardModel> GetAvailableActions(Player player)
		{
			return (from card in player.Hand where card.Is(CardType.Action) select card).ToList();
		}

		protected static double Lerp(double low, double high, int lowCount, int highCount, int count, bool reverse)
		{
			count = Math.Max(lowCount, count);
			count = Math.Min(highCount, count);
			double p = (count - lowCount) / (double)(lowCount + highCount);
			if (reverse) p = 1.0 - p;
			return low + (high - low) * p;
		}
	}

	public class GarboAI : GarboBaseStrategy
	{
		public GarboAI(GameViewModel gameViewModel)
			: base(gameViewModel.GameModel)
		{
		}

		public override string Name
		{
			get { return "Garbo AI"; }
		}

		protected override double[] Weights
		{
			get { return GarboAI.weights; }
		}

		private static double[] weights = new double[]
		{
			1.3325,-3.85,2.134,-3.1275,0.762,-1.119,-0.7675,1.8025,-2.6705,0.6295,-0.994,
		};
	}
}
