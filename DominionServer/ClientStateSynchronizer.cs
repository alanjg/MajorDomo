using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using System.Diagnostics;
using System.ComponentModel;
using Dominion.Network;
using System.Collections.ObjectModel;

namespace DominionServer
{
	public static class CollectionExtensions
	{
		public static int FindIndex<T>(this ObservableCollection<T> collection, Func<T, bool> func)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				if (func(collection[i]))
				{
					return i;
				}
			}
			return -1;
		}
	}

	public class ClientState
	{
		private GameModel serverState;
		private Connection connection;

		public ClientState(Connection connection, GameModel gameModel)
		{
			this.serverState = gameModel;
			gameModel.TurnStarted += gameModel_TurnStarted;
			gameModel.TextLog.LogUpdated += log_LogUpdated;
			this.connection = connection;
			int i = 0;
			foreach (Player player in serverState.Players)
			{
				player.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(playerState_PropertyChanged);
				player.Hand.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Hand_CollectionChanged);
				player.Discard.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Discard_CollectionChanged);
				player.Cleanup.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Cleanup_CollectionChanged);
				player.Played.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Played_CollectionChanged);
				player.Duration.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Duration_CollectionChanged);
				player.Bought.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Bought_CollectionChanged);
				player.GainedLastTurn.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(GainedLastTurn_CollectionChanged);
				player.NativeVillageMat.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(NativeVillageMat_CollectionChanged);
				player.IslandMat.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(IslandMat_CollectionChanged);
				player.Tavern.CollectionChanged += Tavern_CollectionChanged;
				player.SetAsideHorseTraders.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SetAsideHorseTraders_CollectionChanged);
				player.SetAsidePrince.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SetAsidePrince_CollectionChanged);
				player.SetAsidePrincePlay.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SetAsidePrincePlay_CollectionChanged);
				player.SetAsideHaven.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SetAsideHaven_CollectionChanged);
				player.PossessionTrash.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PossessionTrash_CollectionChanged);

				SendCardState(i, "Hand", player.Hand);
				SendCardState(i, "Discard", player.Discard);
				SendCardState(i, "Cleanup", player.Cleanup);
				SendCardState(i, "Played", player.Played);
				SendCardState(i, "Duration", player.Duration);
				SendCardState(i, "Bought", player.Bought);
				SendCardState(i, "GainedLastTurn", player.GainedLastTurn);
				SendCardState(i, "NativeVillageMat", player.NativeVillageMat);
				SendCardState(i, "IslandMat", player.IslandMat);
				SendCardState(i, "Tavern", player.Tavern);
				SendCardState(i, "SetAsideHorseTraders", player.SetAsideHorseTraders);
				SendCardState(i, "SetAsidePrince", player.SetAsidePrince);
				SendCardState(i, "SetAsidePrincePlay", player.SetAsidePrincePlay);
				SendCardState(i, "SetAsideHaven", player.SetAsideHaven);
				SendCardState(i, "PossessionTrash", player.PossessionTrash);

				this.connection.SendPlayerState(i, "Actions", player.Actions.ToString());
				this.connection.SendPlayerState(i, "Buys", player.Buys.ToString());
				this.connection.SendPlayerState(i, "Coin", player.Coin.ToString());
				this.connection.SendPlayerState(i, "CoinTokens", player.CoinTokens.ToString());
				this.connection.SendPlayerState(i, "Potions", player.Potions.ToString());
				this.connection.SendPlayerState(i, "HasOutpostTurn", player.HasOutpostTurn.ToString());
				this.connection.SendPlayerState(i, "IsOutpostTurn", player.IsOutpostTurn.ToString());
				this.connection.SendPlayerState(i, "IsPossessionTurn", player.IsPossessionTurn.ToString());
				this.connection.SendPlayerState(i, "PossessionTurns", player.PossessionTurns.ToString());
				this.connection.SendPlayerState(i, "HasUsedAlms", player.HasUsedAlms.ToString());
				this.connection.SendPlayerState(i, "HasMinusOneCoinToken", player.HasMinusOneCoinToken.ToString());
				this.connection.SendPlayerState(i, "HasUsedBorrow", player.HasUsedAlms.ToString());
				this.connection.SendPlayerState(i, "HasMinusOneCardToken", player.HasMinusOneCoinToken.ToString());
				this.connection.SendPlayerState(i, "DeferredCoin", player.DeferredCoin.ToString());
				this.connection.SendPlayerState(i, "ExpeditionCount", player.ExpeditionCount.ToString());
				this.connection.SendPlayerState(i, "JourneyTokenIsFaceUp", player.JourneyTokenIsFaceUp.ToString());
				this.connection.SendPlayerState(i, "PirateShipTokens", player.PirateShipTokens.ToString());
				this.connection.SendPlayerState(i, "VPChips", player.VPChips.ToString());

				i++;
			}

			foreach (Pile pile in serverState.SupplyPiles)
			{
				pile.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(pile_PropertyChanged);
				this.connection.SendPileState(pile.Card.ID, "Contrabanded", pile.Contrabanded.ToString());
				this.connection.SendPileState(pile.Card.ID, "Cost", pile.Cost.ToString());
				this.connection.SendPileState(pile.Card.ID, "CostsPotion", pile.CostsPotion.ToString());
				this.connection.SendPileState(pile.Card.ID, "Count", pile.Count.ToString());
				this.connection.SendPileState(pile.Card.ID, "EmbargoCount", pile.EmbargoCount.ToString());
				this.connection.SendPileState(pile.Card.ID, "TradeRouteCount", pile.TradeRouteCount.ToString());
				this.connection.SendPileState(pile.Card.ID, "TopCard", pile.TopCard != null ? pile.TopCard.ID : "Blank");
			}

			foreach (Pile pile in serverState.ExtraPiles)
			{
				pile.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(pile_PropertyChanged);
				this.connection.SendPileState(pile.Card.ID, "Contrabanded", pile.Contrabanded.ToString());
				this.connection.SendPileState(pile.Card.ID, "Cost", pile.Cost.ToString());
				this.connection.SendPileState(pile.Card.ID, "CostsPotion", pile.CostsPotion.ToString());
				this.connection.SendPileState(pile.Card.ID, "Count", pile.Count.ToString());
				this.connection.SendPileState(pile.Card.ID, "EmbargoCount", pile.EmbargoCount.ToString());
				this.connection.SendPileState(pile.Card.ID, "TradeRouteCount", pile.TradeRouteCount.ToString());
				this.connection.SendPileState(pile.Card.ID, "TopCard", pile.TopCard != null ? pile.TopCard.ID : "Blank");
			}
			
			this.serverState.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(serverState_PropertyChanged);
			this.connection.SendGameState("Bane", this.serverState.Bane.ID);
			this.connection.SendGameState("CurrentPhase", this.serverState.CurrentPhase.ToString());
			this.connection.SendGameState("CurrentPlayerIndex", this.serverState.CurrentPlayerIndex.ToString());
			this.connection.SendGameState("GameOver", this.serverState.GameOver.ToString());
			this.connection.SendGameState("GameStarted", this.serverState.GameStarted.ToString());
			this.connection.SendGameState("TradeRouteCount", this.serverState.TradeRouteCount.ToString());
			this.connection.SendGameState("TurnCount", this.serverState.TurnCount.ToString());

			this.serverState.Prizes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Prizes_CollectionChanged);
			SendCardState(0, "Prizes", this.serverState.Prizes);
			this.serverState.Trash.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Trash_CollectionChanged);
			SendCardState(0, "Trash", this.serverState.Trash);
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.SetupComplete;
			this.connection.SendMessage(message);
		}

		private void gameModel_TurnStarted(object sender, EventArgs e)
		{
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.StartTurn;
			this.connection.SendMessage(message);
		}

		private void log_LogUpdated(object sender, LogUpdatedEventArgs e)
		{
			LogInfo log = new LogInfo();
			log.Log = e.Text;
			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = GameMessages.GamePrefix;
			message.MessageType = GameMessages.Log;
			message.MessageContent = NetworkSerializer.Serialize(log);
			this.connection.SendMessage(message);
		}

		private void SendCardState(int playerIndex, string cardPile, IEnumerable<CardModel> cards)
		{
			this.connection.SendCardState(playerIndex, cardPile, cards.Select(c => c.ID).ToList());
		}

		private void Hand_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.Hand == sender), "Hand", (IEnumerable<CardModel>)sender);
		}

		private void Discard_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.Discard == sender), "Discard", (IEnumerable<CardModel>)sender);
		}

		private void Cleanup_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.Cleanup == sender), "Cleanup", (IEnumerable<CardModel>)sender);
		}

		private void Played_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.Played == sender), "Played", (IEnumerable<CardModel>)sender);
		}

		private void Duration_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.Duration == sender), "Duration", (IEnumerable<CardModel>)sender);
		}

		private void Bought_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.Bought == sender), "Bought", (IEnumerable<CardModel>)sender);
		}

		private void GainedLastTurn_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.GainedLastTurn == sender), "GainedLastTurn", (IEnumerable<CardModel>)sender);
		}

		private void NativeVillageMat_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.NativeVillageMat == sender), "NativeVillageMat", (IEnumerable<CardModel>)sender);
		}

		private void IslandMat_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.IslandMat == sender), "IslandMat", (IEnumerable<CardModel>)sender);
		}

		private void Tavern_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.Tavern == sender), "Tavern", (IEnumerable<CardModel>)sender);
		}

		private void SetAsideHorseTraders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.SetAsideHorseTraders == sender), "SetAsideHorseTraders", (IEnumerable<CardModel>)sender);
		}

		private void SetAsidePrince_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.SetAsidePrince == sender), "SetAsidePrince", (IEnumerable<CardModel>)sender);
		}

		private void SetAsidePrincePlay_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.SetAsidePrincePlay == sender), "SetAsidePrincePlay", (IEnumerable<CardModel>)sender);
		}

		private void SetAsideHaven_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.SetAsideHaven == sender), "SetAsideHaven", (IEnumerable<CardModel>)sender);
		}

		private void PossessionTrash_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(this.serverState.Players.FindIndex(p => p.PossessionTrash == sender), "PossessionTrash", (IEnumerable<CardModel>)sender);
		}

		private void Trash_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(0, "Trash", (IEnumerable<CardModel>)sender);
		}

		private void Prizes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SendCardState(0, "Prizes", (IEnumerable<CardModel>)sender);
		}

		private void pile_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Pile pile= (Pile)sender;
			string value = string.Empty;
			switch (e.PropertyName)
			{
				case "Count":
					value = pile.Count.ToString();
					break;
				case "EmbargoCount":
					value = pile.EmbargoCount.ToString();
					break;
				case "TradeRouteCount":
					value = pile.TradeRouteCount.ToString();
					break;
				case "Cost":
					value = pile.Cost.ToString();
					break;
				case "CostsPotion":
					value = pile.CostsPotion.ToString();
					break;
				case "Contrabanded":
					value = pile.Contrabanded.ToString();
					break;
				case "TopCard":
					value = pile.TopCard.ToString();
					break;
				default:
					Console.WriteLine("Unknown Pile property: " + e.PropertyName);
					return;
			}
			this.connection.SendPileState(pile.Card.ID, e.PropertyName, value);
		}

		private void playerState_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Player player = (Player)sender;
			int index = this.serverState.Players.IndexOf(player);
			string value = string.Empty;
			switch (e.PropertyName)
			{
				case "Actions":
					value = player.Actions.ToString();
					break;
				case "Buys":
					value = player.Buys.ToString();
					break;
				case "Coin":
					value = player.Coin.ToString();
					break;
				case "CoinTokens":
					value = player.CoinTokens.ToString();
					break;
				case "Potions":
					value = player.Potions.ToString();
					break;
				case "VPChips":
					value = player.VPChips.ToString();
					break;
				case "PirateShipTokens":
					value = player.PirateShipTokens.ToString();
					break;
				case "HasOutpostTurn":
					value = player.HasOutpostTurn.ToString();
					break;
				case "IsOutpostTurn":
					value = player.IsOutpostTurn.ToString();
					break;
				case "IsPossessionTurn":
					value = player.IsPossessionTurn.ToString();
					break;
				case "PossessionTurns":
					value = player.PossessionTurns.ToString();
					break;
				case "HasUsedAlms":
					value = player.HasUsedAlms.ToString();
					break;
				case "HasMinusOneCoinToken":
					value = player.HasMinusOneCoinToken.ToString();
					break;
				case "HasUsedBorrow":
					value = player.HasUsedBorrow.ToString();
					break;
				case "HasMinusOneCardToken":
					value = player.HasMinusOneCardToken.ToString();
					break;
				case "DeferredCoin":
					value = player.DeferredCoin.ToString();
					break;
				case "ExpeditionCount":
					value = player.ExpeditionCount.ToString();
					break;
				case "JourneyTokenIsFaceUp":
					value = player.JourneyTokenIsFaceUp.ToString();
					break;

				default:
					Console.WriteLine("Unknown Player property: " + e.PropertyName);
					return;
			}
			this.connection.SendPlayerState(index, e.PropertyName, value);
		}

		private void serverState_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			GameModel gameModel = (GameModel)sender;
			string value = string.Empty;
			switch(e.PropertyName)
			{
				case "TurnCount":
					value = gameModel.TurnCount.ToString();
					break;
				case "CurrentPhase":
					value = gameModel.CurrentPhase.ToString();
					break;
				case "CurrentPlayerIndex":
					value = gameModel.CurrentPlayerIndex.ToString();
					break;
				case "GameStarted":
					value = gameModel.GameStarted.ToString();
					break;
				case "GameOver":
					value = gameModel.GameOver.ToString();
					break;
				case "TradeRouteCount":
					value = gameModel.TradeRouteCount.ToString();
					break;
				case "Bane":
					value = gameModel.Bane.ID;
					break;
				default:
					Console.WriteLine("Unknown GameModel property:" + e.PropertyName);
					return;
			}
			this.connection.SendGameState(e.PropertyName, value);
		}
	}

	public class ClientStateSynchronizer
	{
		private List<Connection> connections;
		private List<ClientState> clientStates;
		public ClientStateSynchronizer(List<Connection> connections, GameModel gameModel)
		{
			this.connections = connections;
			this.clientStates = new List<ClientState>();
			foreach (Connection connection in connections)
			{
				clientStates.Add(new ClientState(connection, gameModel));
			}
		}
	}
}
