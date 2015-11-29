using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Actions;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public sealed class Estate : BaseCardModel
	{
		public Estate()
		{
			this.Name = "Estate";
			this.Type = CardType.Victory;
			this.Cost = 2;
			this.Points = 1;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Estate();
		}
	}

	public sealed class Duchy : BaseCardModel
	{
		public Duchy()
		{
			this.Name = "Duchy";
			this.Type = CardType.Victory;
			this.Cost = 5;
			this.Points = 3;
		}
		private static string[] choices = new string[] { "Yes", "No" };
		public override void OnGain(GameModel gameModel, Player player)
		{
			Pile pile = gameModel.SupplyPiles.FirstOrDefault(p => p.Card is Duchess);
			if (pile != null && pile.Count > 0)
			{
				int choice = player.Chooser.ChooseOneEffect(EffectChoiceType.GainDuchess, "Do you want to gain a duchess?", choices, choices);
				if (choice == 0)
				{
					player.GainCard(pile);
				}
			}
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Duchy();
		}
	}

	public sealed class Province : BaseCardModel
	{
		public Province()
		{
			this.Name = "Province";
			this.Type = CardType.Victory;
			this.Cost = 8;
			this.Points = 6;
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			foreach (Player otherPlayer in gameModel.Players)
			{
				if (player != otherPlayer)
				{
					IEnumerable<CardModel> reactions = (from c in otherPlayer.Hand where c.Is(CardType.Reaction) && c.ReactionTrigger == Dominion.ReactionTrigger.OpponentGainedProvince select c);
					
					IEnumerable<CardModel> choices = otherPlayer.Chooser.ChooseSeveralCards(CardChoiceType.FoolsGold, "You may trash fool's gold to gain a gold", ChoiceSource.FromHand, 0, reactions.Count(), reactions);
					foreach(CardModel choice in choices.ToList())
					{
						otherPlayer.Trash(choice);
						otherPlayer.GainCard(typeof(Gold), GainLocation.TopOfDeck);
					}
				}
			}
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Province();
		}
	}


	public sealed class Colony : ProsperityCardModel
	{
		public Colony()
		{
			this.Name = "Colony";
			this.Type = CardType.Victory;
			this.Cost = 11;
			this.Points = 10;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Colony();
		}
	}

	public sealed class Curse : BaseCardModel
	{
		public Curse()
		{
			this.Name = "Curse";
			this.Type = CardType.Curse;
			this.Cost = 0;
			this.Points = -1;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Curse();
		}
	}
}
