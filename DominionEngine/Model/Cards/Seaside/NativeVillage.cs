using System;
using System.Collections.Generic;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class NativeVillage : SeasideCardModel
	{
		private static string[] effectChoices = new string[] { "PutCardOnMat", "PutMatInHand" };
		private static string[] effectDescriptions = new string[] { "Draw a card and place on mat", "Put mat contents in hand" };

		public NativeVillage()
		{
			this.Type = CardType.Action;
			this.Name = "Native Village";
			this.Cost = 2;
			this.Actions = 2;
		}
		
		public override void Play(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(EffectChoiceType.NativeVillage, gameModel.CurrentPlayer.NativeVillageMat, "Do you want to put the cards from your Native Village into your hand?", effectChoices, effectDescriptions);
			if (choice == 1)
			{
				gameModel.CurrentPlayer.PutNativeVillageCardsIntoHand();
			}
			else
			{
				gameModel.CurrentPlayer.PutCardOnNativeVillage();
			}
		}

		public override CardModel Clone()
		{
			return new NativeVillage();
		}
	}
}