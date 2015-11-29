using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class PlayerInfo
	{
		public PlayerInfo()
		{
			this.Players = new List<string>();
		}

		public List<string> Players { get; set; }
	}
}
