using System;

namespace Dominion.Model.Actions
{
	public abstract class IntrigueCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Intrigue"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Intrigue; }
		}
	}
}
