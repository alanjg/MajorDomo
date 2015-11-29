using System;

namespace Dominion.Model.Actions
{
	public abstract class BaseCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Base"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Base; }
		}
	}
}
