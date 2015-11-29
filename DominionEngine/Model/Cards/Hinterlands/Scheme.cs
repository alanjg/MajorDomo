using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Scheme : HinterlandsCardModel
	{
		public Scheme()
		{
			this.Name = "Scheme";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.Cards = 1;
			this.Actions = 1;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.AddSchemeEffect();
		}

		public override CardModel Clone()
		{
			return new Scheme();
		}
	}
}
