using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public sealed class Princess : CornucopiaCardModel
	{
		public Princess()
		{
			this.Name = "Princess";
			this.Type = CardType.Action | CardType.Prize;
			this.Cost = 0;
			this.Buys = 1;
		}

		private class PrincessCardModifier : CardModifier
		{
			public override int GetCost(CardModel cardModel, int cost)
			{
				return Math.Max(cost - 2, 0);
			}
		}

		public override void Play(GameModel gameModel)
		{
			if (!gameModel.CardModifiers.Any(modifier => modifier.GetType() == typeof(PrincessCardModifier)))
			{
				gameModel.AddCardModifier(new PrincessCardModifier());
			}
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Princess();
		}
	}
}
