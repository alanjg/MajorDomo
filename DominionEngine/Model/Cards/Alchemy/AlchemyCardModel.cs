using System;

namespace Dominion.Model.Actions
{
	public abstract class AlchemyCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Alchemy"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Alchemy; }
		}
	}
}
