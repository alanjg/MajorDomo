using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Pillage : DarkAgesCardModel
	{
		public Pillage()
		{
			this.Name = "Pillage";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 5;
		}

		public override void Play(GameModel gameModel)
		{
			gameModel.CurrentPlayer.Trash(this);			
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			foreach (Player player in attackedPlayers)
			{
				if (player.Hand.Count >= 5)
				{
					CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.ForceDiscard, "Choose a card for " + player.Name + " to discard", Chooser.ChoiceSource.None, player.Hand);
					player.DiscardCard(choice);
				}
			}
		}

		public override void PlayPostAttack(GameModel gameModel)
		{
			gameModel.CurrentPlayer.GainCard(typeof(Spoils));
			gameModel.CurrentPlayer.GainCard(typeof(Spoils));
		}

		public override CardModel Clone()
		{
			return new Pillage();
		}
	}
}