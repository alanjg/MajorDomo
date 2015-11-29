using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Fugitive : AdventuresCardModel
	{
		private static string[] exchangeChoices = new string[] { "Exchange", "Keep" };
		public Fugitive()
		{
			this.Name = "Fugitive";
			this.Type = CardType.Action | CardType.Traveller;
			this.Cost = 4;
			
			this.Actions = 1;
			this.Cards = 2;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.DiscardCards(1);
		}

		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ExchangeFugitiveForDisiple, "You may exchange Fugitive for a Disciple", exchangeChoices, exchangeChoices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.Played.Remove(this);
				gameModel.PileMap[typeof(Fugitive)].PutCardOnPile(this);
				gameModel.CurrentPlayer.GainCard(typeof(Disciple));
			}
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
			return new Fugitive();
		}
	}
}
