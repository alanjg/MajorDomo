using System;

namespace Dominion.Model.Actions
{
	public abstract class AdventuresCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Adventures"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Adventures; }
		}
	}
}
