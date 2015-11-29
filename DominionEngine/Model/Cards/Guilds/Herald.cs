using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Herald : GuildsCardModel
	{
		public Herald()
		{
			this.Name = "Herald";
			this.Type = CardType.Action;
			this.Cost = 4;
			this.Actions = 1;
			this.Cards = 1;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel top = gameModel.CurrentPlayer.DrawCard();
			if (top != null)
			{
				gameModel.CurrentPlayer.RevealCard(top);
				if (top.Is(CardType.Action))
				{
					gameModel.CurrentPlayer.Play(top, false, true);
				}
				else
				{
					gameModel.CurrentPlayer.DiscardCard(top);
				}
			}
		}

		public override void OnBuy(GameModel gameModel)
		{
			int amount = this.Overpay(gameModel, EffectChoiceType.HeraldPay, "You may overpay for Herald");
			for (int i = 0; i < amount; i++)
			{
				CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.PutOnDeckFromDiscard, "Put a card from the discard on top of your deck", Chooser.ChoiceSource.None, gameModel.CurrentPlayer.Discard);
				if (card != null)
				{
					gameModel.CurrentPlayer.RemoveFromDiscard(card);
					gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Herald();
		}
	}
}
