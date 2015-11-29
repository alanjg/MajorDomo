using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net;

namespace Dominion.Network
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
		}

		private GameModel gameModel;
		public ServerStateSynchronizer(GameModel gameModel)
		{
			this.gameModel = gameModel;
		}

		public void HandleClientStateMessage(NetworkMessage message)
		{
			switch (message.MessageType)
			{
				case GameMessages.SupplyPileInfo:
					SupplyPileInfo supplyPileInfo = NetworkSerializer.Deserialize<SupplyPileInfo>(message.MessageContent);
					foreach (string pile in supplyPileInfo.Piles)
					{
						this.gameModel.SupplyPiles.Add(new Pile(0, this.gameModel, CardModelFactory.GetCardModel(pile).GetType()));
					}
					break;
				case GameMessages.ExtraPileInfo:
					SupplyPileInfo extraPileInfo = NetworkSerializer.Deserialize<SupplyPileInfo>(message.MessageContent);
					foreach (string pile in extraPileInfo.Piles)
					{
						this.gameModel.ExtraPiles.Add(new Pile(0, this.gameModel, CardModelFactory.GetCardModel(pile).GetType()));
					}
					break;
				case GameMessages.PlayerInfo:
					PlayerInfo playerInfo = NetworkSerializer.Deserialize<PlayerInfo>(message.MessageContent);
					foreach (string player in playerInfo.Players)
					{
						this.gameModel.Players.Add(new Player(player, new NetworkedStrategy(gameModel), gameModel));
					}
					break;
				case GameMessages.PlayerState:
					PlayerStateInfo playerState = NetworkSerializer.Deserialize<PlayerStateInfo>(message.MessageContent);
					
					Player p = this.gameModel.Players[playerState.PlayerIndex];
					switch (playerState.PropertyName)
					{
						case "Actions":
							p.Actions = int.Parse(playerState.Value);
							break;
						case "Buys":
							p.Buys = int.Parse(playerState.Value);
							break;
						case "Coin":
							p.Coin = int.Parse(playerState.Value);
							break;
						case "CoinTokens":
							p.CoinTokens = int.Parse(playerState.Value);
							break;
						case "Potions":
							p.Potions = int.Parse(playerState.Value);
							break;
						case "VPChips":
							p.VPChips = int.Parse(playerState.Value);
							break;
						case "PirateShipTokens":
							p.PirateShipTokens = int.Parse(playerState.Value);
							break;
						case "HasOutpostTurn":
							p.HasOutpostTurn = bool.Parse(playerState.Value);
							break;
						case "IsOutpostTurn":
							p.IsOutpostTurn = bool.Parse(playerState.Value);
							break;
						case "IsPossessionTurn":
							p.IsPossessionTurn = bool.Parse(playerState.Value);
							break;
						case "PossessionTurns":
							p.PossessionTurns = int.Parse(playerState.Value);
							break;
						case "HasUsedAlms":
							p.HasUsedAlms = bool.Parse(playerState.Value);
							break;
						case "HasMinusOneCoinToken":
							p.HasMinusOneCoinToken = bool.Parse(playerState.Value);
							break;
						case "HasUsedBorrow":
							p.HasUsedBorrow = bool.Parse(playerState.Value);
							break;
						case "HasMinusOneCardToken":
							p.HasMinusOneCardToken = bool.Parse(playerState.Value);
							break;
						case "DeferredCoin":
							p.DeferredCoin = int.Parse(playerState.Value);
							break;
						case "ExpeditionCount":
							p.ExpeditionCount= int.Parse(playerState.Value);
							break;
						case "JourneyTokenIsFaceUp":
							p.JourneyTokenIsFaceUp = bool.Parse(playerState.Value);
							break;
						default:
							Debug.Assert(false, "Unknown Player property: " + playerState.Value);
							return;
					}
					break;
				case GameMessages.PileState:
					PileStateInfo pileState = NetworkSerializer.Deserialize<PileStateInfo>(message.MessageContent);
					Pile p2 = this.gameModel.SupplyPiles.FirstOrDefault(p3 => p3.Card.ID == pileState.PileName);
					if (p2 == null)
					{
						p2 = this.gameModel.ExtraPiles.FirstOrDefault(p3 => p3.Card.ID == pileState.PileName);
					}
					switch (pileState.PropertyName)
					{
						case "Count":
							p2.Count = int.Parse(pileState.Value);
							break;
						case "EmbargoCount":
							p2.EmbargoCount = int.Parse(pileState.Value);
							break;
						case "TradeRouteCount":
							p2.TradeRouteCount = int.Parse(pileState.Value);
							break;
						case "Cost":
							p2.Cost = int.Parse(pileState.Value);
							break;
						case "CostsPotion":
							p2.CostsPotion = bool.Parse(pileState.Value);
							break;
						case "Contrabanded":
							p2.Contrabanded = bool.Parse(pileState.Value);
							break;
						case "TopCard":
							if (pileState.Value == "Blank")
							{
								p2.TopCard = null;
							}
							else
							{
								p2.TopCard = CardModelFactory.GetCardModel(pileState.Value);
							}
							break;
						default:
							Debug.Assert(false, "Unknown Pile property: " + pileState.PropertyName);
							break;
					}
					break;
				case GameMessages.GameState:
					GameStateInfo gameState = NetworkSerializer.Deserialize<GameStateInfo>(message.MessageContent);
					switch (gameState.PropertyName)
					{
						case "TurnCount":
							this.gameModel.TurnCount = int.Parse(gameState.Value);
							break;
						case "CurrentPhase":
							this.gameModel.CurrentPhase = (GamePhase)Enum.Parse(typeof(GamePhase), gameState.Value, true);
							break;
						case "CurrentPlayerIndex":
							this.gameModel.CurrentPlayerIndex = int.Parse(gameState.Value);
							break;
						case "GameStarted":
							this.gameModel.GameStarted = bool.Parse(gameState.Value);
							break;
						case "GameOver":
							this.gameModel.GameOver = bool.Parse(gameState.Value);
							break;
						case "TradeRouteCount":
							this.gameModel.TradeRouteCount = int.Parse(gameState.Value);
							break;
						case "Bane":
							this.gameModel.Bane = CardModelFactory.GetCardModel(gameState.Value);
							break;
						default:
							Debug.Assert(false, "Unknown GameModel property: " + gameState.PropertyName);
							break;
					}
					break;
				case GameMessages.CardPileState:
					{
						CardPileStateInfo cardPileState = NetworkSerializer.Deserialize<CardPileStateInfo>(message.MessageContent);
						Player player = this.gameModel.Players[cardPileState.PlayerIndex];
						ObservableCollection<CardModel> cardPile = null;
						switch (cardPileState.CardPileName)
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
							case "Tavern":
								cardPile = player.Tavern;
								break;
							case "SetAsideHorseTraders":
								cardPile = player.SetAsideHorseTraders;
								break;
							case "SetAsideHaven":
								cardPile = player.SetAsideHaven;
								break;
							case "SetAsidePrince":
								cardPile = player.SetAsidePrince;
								break;
							case "SetAsidePrincePlay":
								cardPile = player.SetAsidePrincePlay;
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
								Debug.Assert(false, "Unknown CardPile: " + cardPileState.CardPileName);
								break;
						}
						if (cardPile != null)
						{
							List<CardModel> newList = cardPileState.Cards.Select(c => CardModelFactory.GetCardModel(c)).ToList();
							for (int i = 0; i < cardPile.Count; i++)
							{
								bool found = false;
								for (int j = 0; j < newList.Count; j++)
								{
									if (newList[j].Name == cardPile[i].Name)
									{
										newList.RemoveAt(j);
										found = true;
										break;
									}
								}
								if (!found)
								{
									cardPile.RemoveAt(i);
									i--;
								}
							}
							foreach (CardModel card in newList)
							{
								cardPile.Add(card);
							}							
						}
						break;
					}
			}
		}
	}
}
