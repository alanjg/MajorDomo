using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using Dominion.Model.Chooser;
using System.Threading;
using System.Diagnostics;
using Dominion.Network;

namespace DominionServer
{
	public class ServerPlayerStrategy : PlayerStrategy
	{
		private string name;
		private Connection connection;
		private ManualResetEvent resetEvent = new ManualResetEvent(true);
		private NetworkMessage gameMessage;
		private class ServerChooser : ChooserBase
		{
			public ServerPlayerStrategy Strategy { get; set; }
			public ServerChooser()
			{
			}

			public override IEnumerable<CardModel> Order(CardOrderType choiceType, string choiceText, IEnumerable<CardModel> choices)
			{
				IEnumerable<string> chosen = this.MakeCardPileChoice(choiceText, from c in choices select c.ID, choices.Count(), choices.Count(), ChoiceSource.None, card:true, order: true);
				List<CardModel> chosenCards = new List<CardModel>();
				Dictionary<string, int> seen = new Dictionary<string, int>();
				foreach (string c in chosen)
				{
					int skip = 0;
					seen.TryGetValue(c, out skip);
					CardModel foundCard = null;
					int counter = skip;
					foreach (CardModel card in choices)
					{
						if (c == card.ID)
						{
							if (counter > 0)
							{
								counter--;
							}
							else
							{
								foundCard = card;
								break;
							}
						}
					}
					seen[c] = skip + 1;
					chosenCards.Add(foundCard);
				}
				return chosenCards;
			}

			public override IEnumerable<CardModel> ChooseCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices)
			{
				IEnumerable<string> chosen = this.MakeCardPileChoice(choiceText, (from c in choices select c.ID), minChoices, maxChoices, source, card: true, order: false);
				List<CardModel> chosenCards = new List<CardModel>();
				Dictionary<string, int> seen = new Dictionary<string, int>();
				foreach (string c in chosen)
				{
					int skip = 0;
					seen.TryGetValue(c, out skip);
					CardModel foundCard = null;
					int counter = skip;
					foreach (CardModel card in choices)
					{
						if (c == card.ID)
						{
							if (counter > 0)
							{
								counter--;
							}
							else
							{
								foundCard = card;
								break;
							}
						}
					}
					seen[c] = skip + 1;
					chosenCards.Add(foundCard);
				}
				return chosenCards;
			}

			public override IEnumerable<Pile> ChoosePiles(CardChoiceType choiceType, string choiceText, int minChoices, int maxChoices, IEnumerable<Pile> choices)
			{
				IEnumerable<string> chosen = this.MakeCardPileChoice(choiceText, (from c in choices select c.Card.ID), minChoices, maxChoices, ChoiceSource.FromPile, card:false, order: false);
				List<Pile> chosenPiles = new List<Pile>();
				Dictionary<string, int> seen = new Dictionary<string, int>();
				foreach (string c in chosen)
				{
					int skip = 0;
					seen.TryGetValue(c, out skip);
					Pile foundPile = null;
					int counter = skip;
					foreach (Pile pile in choices)
					{
						if (c == pile.Card.ID)
						{
							if (counter > 0)
							{
								counter--;
							}
							else
							{
								foundPile = pile;
								break;
							}
						}
					}
					seen[c] = skip + 1;
					chosenPiles.Add(foundPile);
				}
				return chosenPiles;
			}

			public override IEnumerable<int> ChooseEffects(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
			{
				IEnumerable<string> chosen = this.MakeEffectChoice(choiceText, minChoices, maxChoices,choices, choiceDescriptions);
				return from c in chosen select Array.IndexOf(choices, c);
			}

			private IEnumerable<string> MakeCardPileChoice(string choiceText, IEnumerable<string> choices, int minChoices, int maxChoices, ChoiceSource choiceSource, bool card, bool order)
			{
				RequestChoiceInfo requestChoiceInfo = new RequestChoiceInfo();
				requestChoiceInfo.ChoiceType = order ? "ORDER" : card ? "CARD" : "PILE";
				requestChoiceInfo.ChoiceText = choiceText;
				requestChoiceInfo.ChoiceSource = choiceSource.ToString();
				requestChoiceInfo.MinChoices = minChoices;
				requestChoiceInfo.MaxChoices = maxChoices;
				requestChoiceInfo.Choices.AddRange(choices);

				NetworkMessage message = new NetworkMessage();
				message.MessageCategory = GameMessages.GamePrefix;
				message.MessageType = GameMessages.RequestChoice;
				message.MessageContent = NetworkSerializer.Serialize(requestChoiceInfo);
				
				this.Strategy.connection.SendMessage(message);

				this.Strategy.resetEvent.Reset();
				this.Strategy.resetEvent.WaitOne();
				Debug.Assert(this.Strategy.gameMessage.MessageType == GameMessages.MakeChoice);
				ChoiceInfo choiceInfo = NetworkSerializer.Deserialize<ChoiceInfo>(this.Strategy.gameMessage.MessageContent);
				return choiceInfo.Choices;
			}

			private IEnumerable<string> MakeEffectChoice(string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
			{
				RequestChoiceInfo requestChoiceInfo = new RequestChoiceInfo();
				requestChoiceInfo.ChoiceType = "EFFECT";
				requestChoiceInfo.ChoiceText = choiceText;
				requestChoiceInfo.MinChoices = minChoices;
				requestChoiceInfo.MaxChoices = maxChoices;
				requestChoiceInfo.Choices.AddRange(choices);
				requestChoiceInfo.ChoiceDescriptions.AddRange(choiceDescriptions);

				NetworkMessage message = new NetworkMessage();
				message.MessageCategory = GameMessages.GamePrefix;
				message.MessageType = GameMessages.RequestChoice;
				message.MessageContent = NetworkSerializer.Serialize(requestChoiceInfo);
				this.Strategy.connection.SendMessage(message);

				this.Strategy.resetEvent.Reset();
				this.Strategy.resetEvent.WaitOne();

				Debug.Assert(this.Strategy.gameMessage.MessageType == GameMessages.MakeChoice);

				ChoiceInfo choiceInfo = NetworkSerializer.Deserialize<ChoiceInfo>(this.Strategy.gameMessage.MessageContent);
				return choiceInfo.Choices;	
			}
		}

		public ServerPlayerStrategy(string name, GameModel gameModel, Connection connection)
			:base(gameModel, new ServerChooser())
		{
			this.name = name;
			this.connection = connection;
			((ServerChooser)this.Chooser).Strategy = this;
			this.connection.GameMessage += new GameMessageEventHandler(connection_GameMessage);
		}

		private void connection_GameMessage(GameMessageEventArgs e)
		{
			this.gameMessage = e.Message;
			this.resetEvent.Set();
		}

		public override string Name
		{
			get { return this.name; }
		}

		public override PlayerAction GetNextAction()
		{
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.RequestAction;
			
			this.connection.SendMessage(message);

			this.resetEvent.Reset();
			this.resetEvent.WaitOne();

			PlayerAction nextAction = new PlayerAction();
			switch (this.gameMessage.MessageType)
			{
				case GameMessages.PlayCard:
					{
						ActionCardInfo playCardInfo = NetworkSerializer.Deserialize<ActionCardInfo>(this.gameMessage.MessageContent);
						nextAction.ActionType = ActionType.PlayCard;
						nextAction.Card = this.Player.Hand.First(c => c.ID == playCardInfo.Card);
					}
					break;
				case GameMessages.BuyCard:
					{
						ActionCardInfo buyCardInfo = NetworkSerializer.Deserialize<ActionCardInfo>(this.gameMessage.MessageContent);
						nextAction.ActionType = ActionType.BuyCard;
						nextAction.Pile = this.GameModel.SupplyPiles.First(p => p.Card.ID == buyCardInfo.Card);
					}
					break;
				case GameMessages.CleanupCard:
					{
						ActionCardInfo cleanupCardInfo = NetworkSerializer.Deserialize<ActionCardInfo>(this.gameMessage.MessageContent);
						nextAction.ActionType = ActionType.CleanupCard;
						nextAction.Card = this.Player.Cleanup.First(c => c.ID == cleanupCardInfo.Card);
					}
					break;
				case GameMessages.EndTurn:
					{
						nextAction.ActionType = ActionType.EndTurn;
					}
					break;
				case GameMessages.BuyPhase:
					{
						nextAction.ActionType = ActionType.EnterBuyPhase;
					}
					break;
				case GameMessages.PlayBasicTreasure:
					{
						nextAction.ActionType = ActionType.PlayBasicTreasures;
					}
					break;
				case GameMessages.PlayCoinTokens:
					{
						nextAction.ActionType = ActionType.PlayCoinTokens;
					}
					break;
			}
			return nextAction;
		}
	}
}
