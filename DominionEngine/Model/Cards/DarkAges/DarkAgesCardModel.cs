using System;

namespace Dominion.Model.Actions
{
	public abstract class DarkAgesCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "DarkAges"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.DarkAges; }
		}
	}
}
