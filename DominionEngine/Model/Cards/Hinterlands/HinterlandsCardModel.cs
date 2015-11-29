using System;

namespace Dominion.Model.Actions
{
	public abstract class HinterlandsCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Hinterlands"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Hinterlands; }
		}
	}
}