using System;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Stonemason : GuildsCardModel
	{
		public Stonemason()
		{
			this.Name = "Stonemason";
			this.Type = CardType.Action;
			this.Cost = 2;
		}
		
		public override void Play(GameModel gameModel)
		{
			CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.TrashForStonemason, "Trash a card from your hand.", ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand);
			if (card != null)
			{
				gameModel.CurrentPlayer.Trash(card);
				for (int i = 0; i < 2; i++)
				{
					Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain a card costing less than $" + gameModel.GetCost(card), 
						gameModel.SupplyPiles.Where(
						p => 
							p.Count > 0 && 
							(p.GetCost() < gameModel.GetCost(card) && (!p.CostsPotion || p.CostsPotion && card.CostsPotion) || 
						(p.GetCost() == gameModel.GetCost(card) && !p.CostsPotion && card.CostsPotion))));
					if (pile != null)
					{
						gameModel.CurrentPlayer.GainCard(pile);
					}
				}
			}
		}

		public override void OnBuy(GameModel gameModel)
		{
			int amount = this.Overpay(gameModel, EffectChoiceType.StonemasonPay, "You may overpay for Stonemason");
			for (int i = 0; i < 2; i++)
			{
				Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.Gain, "Gain an action card costing $" + amount, gameModel.SupplyPiles.Where(p => p.GetCost() == amount && !p.CostsPotion && p.Count > 0 && p.Card.Is(CardType.Action)));
				if (pile != null)
				{
					gameModel.CurrentPlayer.GainCard(pile);
				}
			}
		}

		public override CardModel Clone()
		{
			return new Stonemason();
		}
	}
}
