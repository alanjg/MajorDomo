using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
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
		public const string SystemPrefix = "SYSTEM";

		// Server to Client
		public const string Authenticate = "AUTHENTICATE";
		public const string SendLobbies = "SENDLOBBIES";
		public const string AddLobby = "ADDLOBBY";
		public const string RemoveLobby = "REMOVELOBBY";

		public const string SendUsers = "SENDUSERS";
		public const string AddUser = "ADDUSER";
		public const string RemoveUser = "REMOVEUSER";
		public const string SendChat = "SENDCHAT";
		public const string ProposeGame = "PROPOSEGAME";
		public const string CancelRequest = "CANCELREQUEST";
		public const string GameStarted = "GAMESTARTED";

		// Client to server
		public const string Connect = "CONNECT";
		public const string Disconnect = "DISCONNECT";
		public const string EnterLobby = "ENTERLOBBY";

		public const string RequestGame = "REQUESTGAME";
		public const string AcceptGame = "ACCEPTGAME";
		public const string DeclineGame = "DECLINEGAME";
	}
}
