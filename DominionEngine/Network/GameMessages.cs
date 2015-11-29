using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public static class GameMessages
	{
		public static string ParseAction(string message, string action)
		{
			if (message.StartsWith(action))
			{
				return message.Substring(action.Length);
			}
			else
			{
				return null;
			}
		}
		public const string GamePrefix = "GAME";
		// Client to server
		public const string PlayCard = "PLAYCARD";
		public const string PlayBasicTreasure = "PLAYBASICTREASURE";
		public const string BuyCard = "BUYCARD";
		public const string CleanupCard = "CLEANUPCARD";
		public const string MakeChoice = "MAKECHOICE";
		public const string BuyPhase = "BUYPHASE";
		public const string EndTurn = "ENDTURN";
		public const string PlayCoinTokens = "PLAYCOINTOKENS";

		// Server to client
		public const string RequestAction = "REQUESTACTION";
		public const string RequestChoice = "REQUESTCHOICE";
		public const string SetupComplete = "SETUPCOMPLETE";
		public const string StartTurn = "STARTTURN";
		public const string TurnEnded = "TURNENDED";
		public const string GameEnded = "GAMEENDED";
		public const string Log = "LOG";

		public const string SupplyPileInfo = "SUPPLYPILEINFO";
		public const string ExtraPileInfo = "EXTRAPILEINFO";
		public const string PlayerInfo = "PLAYERINFO";

		public const string PileState = "PILESTATE";
		public const string PlayerState = "PLAYERSTATE";
		public const string GameState = "GAMESTATE";
		public const string CardPileState = "CARDPILESTATE";
	}
}
