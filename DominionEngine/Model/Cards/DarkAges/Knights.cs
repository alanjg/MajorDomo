using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Knights : DarkAgesCardModel
	{
		public static IEnumerable<CardModel> AllKnights = new CardModel[] { new SirBailey(), new SirDestry(), new SirMartin(), new SirMichael(), new SirVander(), new DameAnna(), new DameJosephine(), new DameMolly(), new DameNatalie(), new DameSylvia() };
		public Knights()
		{
			this.Name = "Knights";
			this.Type = CardType.Action | CardType.Attack | CardType.Knight;
			this.Cost = 5;
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			bool hitKnight = false;
			foreach (Player player in attackedPlayers)
			{
				IEnumerable<CardModel> cards = player.DrawCards(2);
				IEnumerable<CardModel> eligible = cards.Where(c => gameModel.GetCost(c) >= 3 && gameModel.GetCost(c) <= 6 && !c.CostsPotion);
				CardModel choice = player.Chooser.ChooseOneCard(CardChoiceType.TrashForKnight, "Choose a card to trash", Chooser.ChoiceSource.None, eligible);
				if (choice != null)
				{
					player.Trash(choice);
					if (choice.Is(CardType.Knight))
					{
						hitKnight = true;
					}
				}
				foreach (CardModel card in cards)
				{
					if (card != choice)
					{
						player.DiscardCard(card);
					}
				}
			}
			if (hitKnight)
			{
				gameModel.CurrentPlayer.Trash(this.ThisAsTrashTarget);
			}
		}

		public override CardModel Clone()
		{
			return new Knights();
		}
	}
}