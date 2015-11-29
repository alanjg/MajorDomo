using System;

namespace Dominion.Model.Actions
{
	public abstract class PromoCardModel : CardModel
	{
		public override string Expansion
		{
			get { return "Promo"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Promo; }
		}
	}
}
