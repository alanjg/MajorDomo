using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using Dominion.Model.Actions;
using Dominion.Model.Chooser;

namespace DominionEngine.AI
{
	public class SimpleChooser : ChooserBase
	{
		public override IEnumerable<CardModel> ChooseCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices)
		{
			return choices.Take(minChoices);
		}

		public override IEnumerable<Pile> ChoosePiles(CardChoiceType choiceType, string choiceText, int minChoices, int maxChoices, IEnumerable<Pile> choices)
		{
			return choices.Take(minChoices);
		}

		public override IEnumerable<int> ChooseEffects(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
		{
			List<int> ret = new List<int>();
			for (int i = 0; i < minChoices; i++)
			{
				ret.Add(i);
			}
			return ret;
		}

		public override IEnumerable<CardModel> Order(CardOrderType choiceType, string choiceText, IEnumerable<CardModel> choices)
		{
			return choices;
		}
	}

	public sealed class SimpleAIStrategy : PlayerStrategy
	{
		public SimpleAIStrategy(GameModel gameModel)
			: base(gameModel, new SimpleChooser())
		{
		}

		public override PlayerAction GetNextAction()
		{
			switch (this.GameModel.CurrentPhase)
			{
				case GamePhase.Action:
					return this.ActionPhase(this.GameModel.CurrentPlayer);

				case GamePhase.Buy:
					return this.BuyPhase(this.GameModel.CurrentPlayer);

				case GamePhase.CleanUp:
					return new PlayerAction() { ActionType = ActionType.EndTurn };
			}
			return new PlayerAction();
		}

		public override string Name
		{
			get { return "Simple AI"; }
		}

		private bool TryBuyCard(Player player, Type cardType, out PlayerAction playerAction)
		{
			Pile pile = null;
			if (this.GameModel.PileMap.TryGetValue(cardType, out pile))
			{
				if (player.Coin >= pile.GetCost() && (player.Potions > 0 || !pile.CostsPotion) && pile.Count > 0)
				{
					playerAction = new PlayerAction() { ActionType = ActionType.BuyCard, Pile = pile };
					return true;
				}
			}
			playerAction = null;
			return false;
		}

		private PlayerAction BuyPhase(Player player)
		{
			if (player.HasBasicTreasures)
			{
				return new PlayerAction() { ActionType = ActionType.PlayBasicTreasures };
			}

			foreach (CardModel card in player.Hand)
			{
				if (card.Is(CardType.Treasure))
				{
					return new PlayerAction() { ActionType = ActionType.PlayCard, Card = card };
				}
			}

			PlayerAction playerAction = null;
			if (this.TryBuyCard(player, typeof(Colony), out playerAction)) { return playerAction; }
			if (this.TryBuyCard(player, typeof(Platinum), out playerAction)) { return playerAction; }
			if (this.TryBuyCard(player, typeof(Province), out playerAction)) { return playerAction; }
			if (this.TryBuyCard(player, typeof(Gold), out playerAction)) { return playerAction; }
			if (this.TryBuyCard(player, typeof(Silver), out playerAction)) { return playerAction; }
			return new PlayerAction() { ActionType = ActionType.EndTurn };
		}

		private PlayerAction ActionPhase(Player player)
		{
			IList<CardModel> availableActions = (from card in player.Hand where card.Is(CardType.Action) select card).ToList();
			if (availableActions.Count == 0)
			{
				return new PlayerAction() { ActionType = ActionType.EnterBuyPhase };
			}
			return new PlayerAction()
			{ 
				ActionType = ActionType.PlayCard,
				Card = availableActions[Randomizer.Next(availableActions.Count)]
			};
		}
	}
}
