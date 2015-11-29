using System;

namespace Dominion.Model.Actions
{
	public abstract class CornucopiaCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Cornucopia"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Cornucopia; }
		}
	}
}
