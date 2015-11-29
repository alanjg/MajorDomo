using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Hero : AdventuresCardModel
	{
		public Hero()
		{
			this.Name = "Hero";
			this.Type = CardType.Action | CardType.Traveller;
			this.Cost = 5;
			
			this.Coins = 2;
		}
		private static string[] exchangeChoices = new string[] { "Exchange", "Keep" };
		
		public override void Play(GameModel gameModel)
		{
			Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(Chooser.CardChoiceType.Gain, "Gain a treasure", gameModel.SupplyPiles.Where(p => p.Count > 0 && p.TopCard.Is(CardType.Treasure)));
			if(pile != null)
			{
				gameModel.CurrentPlayer.GainCard(pile);
			}
		}

		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ExchangeHeroForChampion, "You may exchange Hero for a Champion", exchangeChoices, exchangeChoices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.Played.Remove(this);
				gameModel.PileMap[typeof(Hero)].PutCardOnPile(this);
				gameModel.CurrentPlayer.GainCard(typeof(Champion));
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
			return new Hero();
		}
	}
}
