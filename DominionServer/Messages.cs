using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionServer
{
	public static class SystemMessages
	{
		public static string GetSystemAction(string message)
		{
			if (message.StartsWith(SystemPrefix))
			{
				return message.Substring(SystemPrefix.Length);
			}
			else
			{
				return null;
			}
		}
		public const string SystemPrefix =	"SYSTEM|";

		// Server to Client
		public const string Authenticate =	"AUTHENTICATE";
		public const string SendUsers =		"SENDUSERS";
		public const string SendChat =		"SENDCHAT";
		public const string ProposeGame =   "PROPOSEGAME";
		public const string CancelRequest = "CANCELREQUEST";
		
		// Client to server
		public const string Connect =		"CONNECT";
		public const string Disconnect =	"DISCONNECT";
		public const string GetChat =		"GETCHAT";
		public const string GetUsers =		"GETUSERS";
		
		public const string RequestGame =   "REQUESTGAME";
		public const string AcceptGame =	"ACCEPTGAME";
		public const string DeclineGame =	"DECLINEGAME";
	}

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
		public const string GamePrefix =	"GAME|";
		// Client to server
		public const string PlayCard =		"PLAYCARD";
		public const string PlayBasicTreasure="PLAYBASICTREASURE";
		public const string BuyCard =		"BUYCARD";
		public const string CleanupCard =	"CLEANUPCARD";
		public const string MakeChoice =	"MAKECHOICE";
		public const string MakeReaction =	"MAKEREACTION";
		public const string BuyPhase =		"BUYPHASE";
		public const string EndTurn =		"ENDTURN";

		// Server to client
		public const string RequestAction = "REQUESTACTION";
		public const string RequestChoice =	"REQUESTCHOICE";
		public const string RequestReaction="REQUESTREACTION";
		public const string EndGame =		"ENDGAME";
		public const string Log =			"LOG";
		
		public const string SupplyPileInfo ="SUPPLYPILEINFO|";
		public const string ExtraPileInfo =	"EXTRAPILEINFO|";
		public const string PlayerInfo =	"PLAYERINFO|";

		public const string PileState =		"PILESTATE|";
		public const string PlayerState =	"PLAYERSTATE|";
		public const string GameState =		"GAMESTATE|";
		public const string CardPileState = "CARDPILESTATE|";
	}
}
