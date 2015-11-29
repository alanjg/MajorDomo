using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using Dominion.Model.Chooser;

namespace DominionEngine.AI
{
	
	public class RandomAIChooser : ChooserBase
	{
		private GameModel gameModel;
		public RandomAIChooser(GameModel gameModel)
		{
			this.gameModel = gameModel;
		}

		public override IEnumerable<CardModel> ChooseCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices)
		{
			maxChoices = Math.Min(maxChoices, choices.Count());
			int howMany = Randomizer.Next(minChoices, maxChoices + 1);
			return choices.OrderBy(c => Randomizer.Next()).Take(howMany);
		}

		public override IEnumerable<int> ChooseEffects(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
		{
			maxChoices = Math.Min(maxChoices, choices.Count());
			int howMany = Randomizer.Next(minChoices, maxChoices + 1);
			List<int> indexes = new List<int>();
			for (int i = 0; i < choices.Count(); i++)
			{
				indexes.Add(i);
			}
			return indexes.OrderBy(c => Randomizer.Next()).Take(howMany);
		}

		public override IEnumerable<Pile> ChoosePiles(CardChoiceType choiceType, string choiceText, int minChoices, int maxChoices, IEnumerable<Pile> choices)
		{
			maxChoices = Math.Min(maxChoices, choices.Count());
			int howMany = Randomizer.Next(minChoices, maxChoices + 1);
			return choices.OrderBy(c => Randomizer.Next()).Take(howMany);
		}

		public override IEnumerable<CardModel> Order(CardOrderType choiceType, string choiceText, IEnumerable<CardModel> choices)
		{
			return choices.OrderBy(c => Randomizer.Next());
		}
	}


	public sealed class RandomAIStrategy : PlayerStrategy
	{
		public RandomAIStrategy(GameModel gameModel)
			: base(gameModel, new RandomAIChooser(gameModel))
		{
		}

		public override string Name
		{
			get { return "Random"; }
		}
		public override PlayerAction GetNextAction()
		{
			Player tempPlayer = this.Player;
			try
			{
				if (this.GameModel.CurrentPlayer.IsPossessionTurn)
				{
					this.Player = this.GameModel.CurrentPlayer;
				}
				switch (this.GameModel.CurrentPhase)
				{
					case GamePhase.Action:
						IEnumerable<CardModel> actions = this.Player.Hand.Where(c => c.Is(CardType.Action));
						int actionsCount = actions.Count();
						int which = Randomizer.Next(actionsCount + 1);
						if (which == actionsCount) return new PlayerAction() { ActionType = ActionType.EnterBuyPhase };
						return new PlayerAction() { ActionType = ActionType.PlayCard, Card = actions.ElementAt(which) };

					case GamePhase.Buy:
						IEnumerable<CardModel> treasures = this.Player.Hand.Where(c => c.Is(CardType.Treasure));
						int treasuresCount = treasures.Count();
						int whichTreasure = Randomizer.Next(treasuresCount + 1);
						if (whichTreasure != treasuresCount) return new PlayerAction() { ActionType = ActionType.PlayCard, Card = treasures.ElementAt(whichTreasure) };

						IEnumerable<Pile> piles = this.GameModel.SupplyPiles.Where(p => p.Cost <= this.Player.Coin && (!p.CostsPotion || this.Player.Potions > 0) && p.Count > 0 && !p.Contrabanded);
						int pileCount = piles.Count();
						int whichPile = Randomizer.Next(pileCount + 1);
						if (whichPile != pileCount) return new PlayerAction() { ActionType = ActionType.BuyCard, Pile = piles.ElementAt(whichPile) };

						return new PlayerAction() { ActionType = ActionType.EndTurn };

					case GamePhase.CleanUp:
						int cleanupCount = this.Player.Cleanup.Count();
						int whichCleanup = Randomizer.Next(cleanupCount + 1);
						if (whichCleanup != cleanupCount) return new PlayerAction() { ActionType = ActionType.CleanupCard, Card = this.Player.Cleanup.ElementAt(whichCleanup) };

						return new PlayerAction() { ActionType = ActionType.EndTurn };
				}
				return new PlayerAction();
			}
			finally
			{
				this.Player = tempPlayer;
			}
		}
	}

}
