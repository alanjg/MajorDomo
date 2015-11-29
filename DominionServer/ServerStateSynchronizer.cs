using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DominionServer
{
	public class ServerStateSynchronizer
	{
		private class NetworkedStrategy : PlayerStrategy
		{
			public NetworkedStrategy(GameModel gameModel)
				: base(gameModel, null)
			{
			}

			public override string Name
			{
				get { return "Network"; }
			}

			public override PlayerAction GetNextAction()
			{
				throw new NotImplementedException();
			}

			public override CardModel React(CardModel source, ReactionTrigger trigger, IEnumerable<CardModel> reactions)
			{
				throw new NotImplementedException();
			}
		}

		private GameModel gameModel;
		public ServerStateSynchronizer(GameModel gameModel)
		{
			this.gameModel = gameModel;
		}

		public void HandleClientStateMessage(string message)
		{
			int splitter = message.IndexOf('|');
			string messageType = message.Substring(0, splitter + 1);
			string[] messageContents = message.Substring(splitter + 1).Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			switch(messageType)
			{
				case GameMessages.SupplyPileInfo:
					foreach(string pile in messageContents)
					{
						this.gameModel.SupplyPiles.Add(new Pile(0, this.gameModel, CardModelFactory.GetCardModel(pile).GetType()));
					}
					break;
				case GameMessages.ExtraPileInfo:
					foreach (string pile in messageContents)
					{
						this.gameModel.ExtraPiles.Add(new Pile(0, this.gameModel, CardModelFactory.GetCardModel(pile).GetType()));
					}
					break;
				case GameMessages.PlayerInfo:
					foreach(string player in messageContents)
					{
						this.gameModel.Players.Add(new Player(player, new NetworkedStrategy(gameModel), gameModel));
					}
					break;
				case GameMessages.PlayerState:
					int playerIndex = int.Parse(messageContents[0]);
					Player p = this.gameModel.Players[playerIndex];
					PlayerState playerState = p.PlayerState;
					switch(messageContents[1])
					{
						case "Actions":
							playerState.Actions = int.Parse(messageContents[2]);
							break;
						case "Buys":
							playerState.Buys = int.Parse(messageContents[2]);
							break;
						case "Coin":
							playerState.Coin = int.Parse(messageContents[2]);
							break;
						case "Potions":
							playerState.Potions = int.Parse(messageContents[2]);
							break;
						case "VPChips":
							playerState.VPChips = int.Parse(messageContents[2]);
							break;
						case "PirateShipTokens":
							playerState.PirateShipTokens = int.Parse(messageContents[2]);
							break;
						case "HasOutpostTurn":
							playerState.HasOutpostTurn = bool.Parse(messageContents[2]);
							break;
						case "IsOutpostTurn":
							playerState.IsOutpostTurn = bool.Parse(messageContents[2]);
							break;
						case "IsPossessionTurn":
							playerState.IsPossessionTurn = bool.Parse(messageContents[2]);
							break;
						case "PossessionTurns":
							playerState.PossessionTurns = int.Parse(messageContents[2]);
							break;
						default:
							Debug.Assert(false, "Unknown PlayerState property: " + messageContents[2]);
							return;
					}
					break;
				case GameMessages.PileState:
					Pile p2 = this.gameModel.SupplyPiles.First(p3 => p3.Card.ID == messageContents[0]);
					PileState pileState = p2.PileState;
					switch(messageContents[1])
					{
						case "Count":
							pileState.Count = int.Parse(messageContents[2]);
							break;
						case "EmbargoCount":
							pileState.EmbargoCount = int.Parse(messageContents[2]);
							break;
						case "TradeRouteCount":
							pileState.TradeRouteCount = int.Parse(messageContents[2]);
							break;
						case "Cost":
							pileState.Cost = int.Parse(messageContents[2]);
							break;
						case "CostsPotion":
							pileState.CostsPotion = bool.Parse(messageContents[2]);
							break;
						case "Contrabanded":
							pileState.Contrabanded = bool.Parse(messageContents[2]);
							break;		
						default:
							Debug.Assert(false, "Unknown PileState property: " + messageContents[1]);
							break;
					}
					break;
				case GameMessages.GameState:
					GameState clientState = this.gameModel.GameState;
					switch(messageContents[0])
					{
						case "TurnCount":
							clientState.TurnCount = int.Parse(messageContents[1]);
							break;
						case "CurrentPhase":
							clientState.CurrentPhase = (GamePhase)Enum.Parse(typeof(GamePhase), messageContents[1], true);
							break;
						case "CurrentPlayerIndex":
							clientState.CurrentPlayerIndex = int.Parse(messageContents[1]);
							break;
						case "GameStarted":
							clientState.GameStarted = bool.Parse(messageContents[1]);
							break;
						case "GameOver":
							clientState.GameOver = bool.Parse(messageContents[1]);
							break;
						case "TradeRouteCount":
							clientState.TradeRouteCount = int.Parse(messageContents[1]);
							break;
						case "Bane":
							clientState.Bane = CardModelFactory.GetCardModel(messageContents[1]);
							break;
						default:
							Debug.Assert(false, "Unknown GameState property: " + messageContents[0]);
							break;
					}
					break;
				case GameMessages.CardPileState:
					{
						string cardPileName = messageContents[1];
						PlayerState player = this.gameModel.Players[int.Parse(messageContents[0])].PlayerState;
						ObservableCollection<CardModel> cardPile = null;
						switch (cardPileName)
						{
							case "Hand":
								cardPile = player.Hand;
								break;
							case "Discard":
								cardPile = player.Discard;
								break;
							case "Cleanup":
								cardPile = player.Cleanup;
								break;
							case "Played":
								cardPile = player.Played;
								break;
							case "Duration":
								cardPile = player.Duration;
								break;
							case "Bought":
								cardPile = player.Bought;
								break;
							case "GainedLastTurn":
								cardPile = player.GainedLastTurn;
								break;
							case "NativeVillageMat":
								cardPile = player.NativeVillageMat;
								break;
							case "IslandMat":
								cardPile = player.IslandMat;
								break;
							case "SetAside":
								cardPile = player.SetAside;
								break;
							case "Prizes":
								cardPile = this.gameModel.Prizes;
								break;
							case "BlackMarket":
								cardPile = this.gameModel.BlackMarket;
								break;
							case "Trash":
								cardPile = this.gameModel.Trash;
								break;
							case "PossessionTrash":
								cardPile = player.PossessionTrash;
								break;
							default:
								Debug.Assert(false, "Unknown CardPile: " + cardPileName);
								break;
						}
						if (cardPile != null)
						{
							cardPile.Clear();
							for (int i = 2; i < messageContents.Length; i++)
							{
								cardPile.Add(CardModelFactory.GetCardModel(messageContents[i]));
							}
						}
						break;
					}
			}
		}
	}
}
