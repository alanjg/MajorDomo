using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class LobbyInfo
	{
		public LobbyInfo()
		{
			this.Lobbies = new List<string>();
		}
		public List<string> Lobbies { get; set; }
	}
}
