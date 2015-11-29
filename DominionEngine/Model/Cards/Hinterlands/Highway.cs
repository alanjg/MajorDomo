using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Highway : HinterlandsCardModel
	{
		public Highway()
		{
			this.Name = "Highway";
			this.Type = CardType.Action;
			this.Cost = 5;
			this.Actions = 1;
			this.Cards = 1;
		}

		private class HighwayCardModifier : CardModifier
		{
			private Highway source;
			public Highway Source { get { return this.source; } }
			public HighwayCardModifier(Highway source)
			{
				this.source = source;
			}

			public override int GetCost(CardModel cardModel, int cost)
			{
				return Math.Max(cost - 1, 0);
			}

			public override CardModifier Clone()
			{
				// Todo, highway doesn't clone correctly.
				return new HighwayCardModifier(this.source);
			}
		}

		public override void Play(GameModel gameModel)
		{
			if (!gameModel.CardModifiers.OfType<HighwayCardModifier>().Any(m => m.Source == this))
			{
				gameModel.AddCardModifier(new HighwayCardModifier(this));
			}			
		}

		public override void OnRemovedFromPlay(GameModel gameModel)
		{
			gameModel.CardModifiers.RemoveAll(c => c is HighwayCardModifier && ((HighwayCardModifier)c).Source == this);
		}

		public override CardModel Clone()
		{
			return new Highway();
		}
	}
}
