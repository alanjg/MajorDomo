using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class CardPileStateInfo
	{
		public CardPileStateInfo()
		{
			this.Cards = new List<string>();
		}

		public int PlayerIndex { get; set; }
		public string CardPileName { get; set; }
		public List<string> Cards { get; set; }
	}
}
