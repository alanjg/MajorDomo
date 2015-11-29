using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Beggar : DarkAgesCardModel
	{
		public Beggar()
		{
			this.Name = "Beggar";
			this.Type = CardType.Action | CardType.Reaction;
			this.Cost = 2;
			this.ReactionTrigger = Dominion.ReactionTrigger.AttackPlayed;
		}
		private static string[] choices = new string[] { "Discard", "DoNotDiscard" };
		private static string[] choiceText = new string[] { "Discard", "Do Not Discard" };
		public override bool ReactToAttack(GameModel gameModel, Player targetPlayer)
		{
			int choice = targetPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DiscardBeggar, "You may discard Beggar to gain two Silvers", choices, choiceText);	
			if (choice == 0)
			{
				targetPlayer.DiscardCard(this);
				targetPlayer.GainCard(typeof(Silver), GainLocation.TopOfDeck);
				targetPlayer.GainCard(typeof(Silver));
			}
			return false;
		}

		public override void Play(GameModel gameModel)
		{
			for (int i = 0; i < 3; i++)
			{
				gameModel.CurrentPlayer.GainCard(typeof(Copper), GainLocation.InHand);
			}
		}

		public override CardModel Clone()
		{
			return new Beggar();
		}
	}
}