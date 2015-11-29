using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Teacher : AdventuresCardModel
	{
		public new const string Name = "Teacher";
		public Teacher()
		{
			base.Name = Teacher.Name;
			this.Type = CardType.Action | CardType.Reserve;
			this.Cost = 6;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.RemoveFromPlayed(this.ThisAsTrashTarget);
			gameModel.CurrentPlayer.PutOnTavern(this.ThisAsTrashTarget);
		}

		public override bool IsKingdomCard
		{
			get
			{
				return false;
			}
		}
		public override CardModel Clone()
		{
			return new Teacher();
		}
	}
}
