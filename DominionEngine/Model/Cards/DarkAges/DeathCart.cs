using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class DeathCart : DarkAgesCardModel
	{
		public DeathCart()
		{
			this.Name = "Death Cart";
			this.Type = CardType.Action | CardType.Looter;
			this.Cost = 4;
			this.Coins = 5;
		}

		public override void Play(GameModel gameModel)
		{
			IEnumerable<CardModel> actions = gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Action));
			CardModel choice = gameModel.CurrentPlayer.Chooser.ChooseZeroOrOneCard(CardChoiceType.TrashForDeathCart, "You may trash an action card from your hand", Chooser.ChoiceSource.FromHand, actions);
			if (choice != null)
			{
				gameModel.CurrentPlayer.Trash(choice);
			}
			else
			{
				gameModel.CurrentPlayer.Trash(this);
			}
		}

		public override void OnGain(GameModel gameModel, Player player)
		{
			player.GainCard(gameModel.Ruins);
			player.GainCard(gameModel.Ruins);
		}

		public override CardModel Clone()
		{
			return new DeathCart();
		}
	}
}