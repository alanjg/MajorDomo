using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Cultist : DarkAgesCardModel
	{
		public Cultist()
		{
			this.Name = "Cultist";
			this.Type = CardType.Action | CardType.Attack | CardType.Looter;
			this.Cost = 5;
			this.Cards = 2;
		}

		private static string[] choices = new string[] { "Yes", "No" };

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				player.GainCard(gameModel.Ruins);
			}
		}

		public override void PlayPostAttack(GameModel gameModel)
		{
			CardModel cultist = gameModel.CurrentPlayer.Hand.FirstOrDefault(c => c is Cultist);
			if (cultist != null)
			{
				int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.Cultist, "You may play a cultist from hand", choices, choices);
				if (choice == 0)
				{
					gameModel.CurrentPlayer.Play(cultist, false, true);
				}
			}
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			owner.Draw(3);
		}

		public override CardModel Clone()
		{
			return new Cultist();
		}
	}
}