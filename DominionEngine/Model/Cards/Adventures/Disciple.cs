using Dominion.Model.Chooser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Disciple : AdventuresCardModel
	{
		public Disciple()
		{
			this.Name = "Disciple";
			this.Type = CardType.Action;
			this.Cost = 3;
			this.CostsPotion = true;
			this.Actions = 1;
			this.Cards = 2;
		}
		private static string[] exchangeChoices = new string[] { "Exchange", "Keep" };
		public override void Play(GameModel gameModel)
		{
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.Disciple, "You may play an action card from your hand twice.", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Action)));
			if (choice != null)
			{
				using (choice.ForceMultipleCardPlayChoice())
				{
					gameModel.CurrentPlayer.Play(choice, false, true);
					gameModel.CurrentPlayer.Play(choice, false, false, " a second time");
				}

				KeyValuePair<Type, Pile> pile = gameModel.PileMap.FirstOrDefault(p => p.Value.Count > 0 && p.Value.TopCard.Name == choice.Name);
				if (pile.Value != null)
				{
					gameModel.CurrentPlayer.GainCard(pile.Value);
				}				
			}
		}

		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.ExchangeDisipleForTeacher, "You may exchange Disciple for a Teacher", exchangeChoices, exchangeChoices);
			if (choice == 0)
			{
				gameModel.CurrentPlayer.Played.Remove(this);
				gameModel.PileMap[typeof(Disciple)].PutCardOnPile(this);
				gameModel.CurrentPlayer.GainCard(typeof(Teacher));
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
			return new Disciple();
		}
	}
}
