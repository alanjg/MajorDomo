using System;
using System.Collections.Generic;
using System.Linq;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class Doctor : GuildsCardModel
	{
		public Doctor()
		{
			this.Name = "Doctor";
			this.Type = CardType.Action;
			this.Cost = 3;
		}

		public override void Play(GameModel gameModel)
		{
			CardModel namedCard = gameModel.CurrentPlayer.Chooser.ChooseOneCard(CardChoiceType.NameACardForDoctor, "Name a card", ChoiceSource.None, gameModel.AllCardsInGame);
			List<CardModel> top = gameModel.CurrentPlayer.DrawCards(3);
			List<CardModel> putBack = new List<CardModel>();
			gameModel.CurrentPlayer.RevealCards(top);
			foreach (CardModel card in top)
			{
				if (card.Name == namedCard.Name)
				{
					gameModel.CurrentPlayer.Trash(card);
				}
				else
				{
					putBack.Add(card);
				}
			}
			foreach (CardModel card in gameModel.CurrentPlayer.Chooser.ChooseOrder(CardOrderType.OrderOnDeck, "Put the rest back in any order(first on top)", putBack).Reverse())
			{
				gameModel.CurrentPlayer.Deck.PlaceOnTop(card);
			}
		}

		private static string[] choices = new string[] { "Trash", "Discard", "PutBack" };
		private static string[] choiceDescriptions = new string[] { "Trash", "Discard", "Put Back" };
		public override void OnBuy(GameModel gameModel)
		{			
			int amount = this.Overpay(gameModel, EffectChoiceType.DoctorPay, "You may overpay for Doctor");
			for (int i = 0; i < amount;i++)
			{
				CardModel card = gameModel.CurrentPlayer.DrawCard();
				if (card == null)
				{
					break;
				}
				int effect = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.DoctorEffect, card, "You see " + card.Name, choices, choiceDescriptions);
				switch (effect)
				{
					case 0: gameModel.CurrentPlayer.Trash(card); break;
					case 1: gameModel.CurrentPlayer.DiscardCard(card); break;
					case 2: gameModel.CurrentPlayer.Deck.PlaceOnTop(card); break;
				}
			}			
		}

		public override CardModel Clone()
		{
			return new Doctor();
		}
	}
}
