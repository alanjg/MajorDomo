using System;

namespace Dominion.Model.Actions
{
	public abstract class ProsperityCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Prosperity"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Prosperity; }
		}
	}
}
