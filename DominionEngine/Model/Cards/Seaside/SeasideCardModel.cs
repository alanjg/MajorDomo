using System;

namespace Dominion.Model.Actions
{
	public abstract class SeasideCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Seaside"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Seaside; }
		}
	}
}
