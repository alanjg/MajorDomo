using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Mercenary : DarkAgesCardModel
	{
		public Mercenary()
		{
			this.Name = "Mercenary";
			this.Type = CardType.Action | CardType.Attack;
			this.Cost = 0;
		}

		public override CardModel Clone()
		{
			Mercenary clone = (Mercenary)base.Clone();
			clone.trashed = this.trashed;
			return clone;
		}
		private static string[] choices = new string[] { "Trash", "Nothing" };
		private static string[] choiceDescriptions = new string[] { "Trash 2 cards", "Do nothing" };
		private bool trashed = false;
		public override void Play(GameModel gameModel)
		{
			this.trashed = false;
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.TrashForMercenary, "You may trash 2 cards from your hand", choices, choiceDescriptions);
			if (choice == 0)
			{
				IList<CardModel> trashChoices = gameModel.CurrentPlayer.Chooser.ChooseSeveralCards(CardChoiceType.Trash, "Trash 2 cards from your hand", Chooser.ChoiceSource.FromHand, 2, 2, gameModel.CurrentPlayer.Hand).ToList();
				foreach (CardModel card in trashChoices)
				{
					gameModel.CurrentPlayer.Trash(card);
				}
				if (trashChoices.Count() == 2)
				{
					gameModel.CurrentPlayer.Draw(2);
					gameModel.CurrentPlayer.AddActionCoin(2);
					this.trashed = true;
				}
			}
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			if (this.trashed)
			{
				foreach (Player player in attackedPlayers)
				{
					player.DiscardTo(3);
				}
			}
			this.trashed = false;
		}

		public override bool IsKingdomCard { get { return false; } }
	}
}