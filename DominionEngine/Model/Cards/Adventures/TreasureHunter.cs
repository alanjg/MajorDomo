using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class TreasureHunter : AdventuresCardModel
	{
		public TreasureHunter()
		{
			this.Name = "Treasure Hunter";
			this.Type = CardType.Action | CardType.Traveller;
			this.Cost = 3;
			this.Actions = 1;
			this.Coins = 1;
		}

		public override void Play(GameModel gameModel)
		{			
			for(int i=0;i<gameModel.RightOfCurrentPlayer.GainedLastTurn.Count;i++)
			{
				gameModel.CurrentPlayer.GainCard(typeof(Silver));
			}				
		}
		
		private static string[] exchangeChoices = new string[] { "Exchange", "Keep" };
		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ExchangeTreasureHunterForWarrior, "You may exchange Treasure Hunter for a Warrior", exchangeChoices, exchangeChoices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.Played.Remove(this);
				gameModel.PileMap[typeof(TreasureHunter)].PutCardOnPile(this);
				gameModel.CurrentPlayer.GainCard(typeof(Warrior));
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
			return new TreasureHunter();
		}
	}
}
