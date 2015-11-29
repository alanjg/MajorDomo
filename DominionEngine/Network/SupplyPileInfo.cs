using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class SupplyPileInfo
	{
		public SupplyPileInfo()
		{
			this.Piles = new List<string>();
		}
		public List<string> Piles { get; set; }
	}
}
