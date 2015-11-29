using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class BlankCard : BaseCardModel
	{
		public BlankCard()
		{
			this.Name = "Blank";
			this.Type = CardType.Action;
		}

		public override bool IsKingdomCard { get { return false; } }
	}
}
