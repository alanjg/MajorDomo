using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Actions
{
	public class Prince : PromoCardModel
	{
		public Prince()
		{
			this.Name = "Prince";
			this.Cost = 8;
			this.Type = CardType.Action;
		}

		private static string[] setAside = new string[] { "Yes", "No" };
		public override void Play(GameModel gameModel)
		{
			int choice = gameModel.CurrentPlayer.Chooser.ChooseOneEffect(Chooser.EffectChoiceType.Prince, "You may set aside Prince", setAside, setAside);
			if (choice == 0)
			{
				bool didSetAside = gameModel.CurrentPlayer.Played.Remove(this);
				if (didSetAside)
				{
					gameModel.CurrentPlayer.SetAsidePrince.Add(this);
					CardModel card = gameModel.CurrentPlayer.Chooser.ChooseOneCard(Chooser.CardChoiceType.Prince, "Choose a card to set aside for Prince", Chooser.ChoiceSource.FromHand, gameModel.CurrentPlayer.Hand.Where(c => c.Is(CardType.Action) && gameModel.GetCost(c) <= 4 && !c.CostsPotion));
					if (card != null)
					{
						gameModel.TextLog.WriteLine(gameModel.CurrentPlayer.Name + " sets aside " + card.Name);
						gameModel.CurrentPlayer.Hand.Remove(card);
						gameModel.CurrentPlayer.SetAsidePrincePlay.Add(card);
						card.PlayedWithPrinceSource = this;
					}
				}				
			}
			base.Play(gameModel);
		}
	}
}
